using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcExample.Models;
using OneOf;
using TrueLayer;
using TrueLayer.Common;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.GetPaymentResponse;

namespace MvcExample.Controllers;

public class PaymentsController : Controller
{
    private readonly ITrueLayerClient _trueLayerClient;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(ITrueLayerClient trueLayerClient, ILogger<PaymentsController> logger)
    {
        _trueLayerClient = trueLayerClient;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Donate(DonateModel donateModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }

        OneOf<Provider.UserSelected, Provider.Preselected> providerSelection = donateModel.UserPreSelectedFilter
            ? new Provider.Preselected(
                providerId: "mock-payments-gb-redirect",
                schemeSelection: new SchemeSelection.Preselected { SchemeId = "faster_payments_service"})
            : new Provider.UserSelected();

        var paymentRequest = new CreatePaymentRequest(
            donateModel.AmountInMajor.ToMinorCurrencyUnit(2),
            Currencies.GBP,
            new PaymentMethod.BankTransfer(
                providerSelection,
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new AccountIdentifier.SortCodeAccountNumber("567890", "12345678"))),
            new PaymentUserRequest(name: donateModel.Name, email: donateModel.Email, dateOfBirth: new DateTime(1999, 1, 1),
                address: new Address("London", "England", "EC1R 4RB", "GB", "1 Hardwick St", "Awesome building")),
            null
        );

        var apiResponse = await _trueLayerClient.Payments.CreatePayment(
            paymentRequest,
            idempotencyKey: Guid.NewGuid().ToString()
        );

        if (!apiResponse.IsSuccessful)
        {
            _logger.LogError("Create TrueLayer payment failed with status code {StatusCode}", apiResponse.StatusCode);

            if (apiResponse.Problem?.Errors != null)
            {
                foreach (var error in apiResponse.Problem.Errors)
                {
                    ModelState.AddModelError("", $"{error.Key}: {error.Value?.FirstOrDefault()}");
                }
            }

            ModelState.AddModelError("", "Payment failed");
            return View("Index");
        }

        return apiResponse.Data.Match<IActionResult>(
            authorizationRequired =>
            {
                // Return Uri must be whitelisted in TrueLayer console
                var returnUri = new Uri(Url.ActionLink("Success"));

                var hppLink = _trueLayerClient.Payments.CreateHostedPaymentPageLink(
                    authorizationRequired.Id,
                    authorizationRequired.ResourceToken,
                    returnUri);
                return Redirect(hppLink);
            },
            authorized =>
            {
                ViewData["Status"] = authorized.Status;
                return View("Pending");
            },
            failed =>
            {
                ViewData["Status"] = failed.Status;
                return View("Failed");
            },
            authorizing =>
            {
                ViewData["Status"] = authorizing.Status;
                return View("Pending");
            });
    }

    [HttpGet]
    public async Task<IActionResult> Complete([FromQuery(Name = "payment_id")] string paymentId)
    {
        if (string.IsNullOrWhiteSpace(paymentId))
            return StatusCode((int)HttpStatusCode.BadRequest);

        var apiResponse = await _trueLayerClient.Payments.GetPayment(paymentId);

        if (!apiResponse.IsSuccessful)
            return Failed(apiResponse.StatusCode.ToString(), null!);

        return apiResponse.Data.Match(
            authRequired => Failed(authRequired.Status, authRequired.PaymentMethod),
            SuccessOrPending,
            SuccessOrPending,
            SuccessOrPending,
            SuccessOrPending,
            failed => Failed(failed.Status, failed.PaymentMethod),
            attemptFailed => Failed(attemptFailed.Status, attemptFailed.PaymentMethod)
        );

        IActionResult SuccessOrPending(PaymentDetails payment)
        {
            ViewData["Status"] = payment.Status;
            SetProviderAndSchemeId(payment.PaymentMethod);
            return View("Success");
        }

        void SetProviderAndSchemeId(OneOf<PaymentMethod.BankTransfer, PaymentMethod.Mandate>? paymentMethod)
        {
            (string providerId, string schemeId) = paymentMethod?.Match(
                bankTransfer => bankTransfer.ProviderSelection.Match(
                    userSelected => (userSelected.ProviderId, userSelected.SchemeId),
                    preselected => (preselected.ProviderId, preselected.SchemeId)
                ),
                mandate => ("unavailable", "unavailable")) ?? ("unavailable", "unavailable");

            ViewData["ProviderId"] = providerId;
            ViewData["SchemeId"] = schemeId;
        }

        IActionResult Failed(string status, OneOf<PaymentMethod.BankTransfer, PaymentMethod.Mandate>? paymentMethod)
        {
            ViewData["Status"] = status;

            SetProviderAndSchemeId(paymentMethod);
            return View("Failed");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}