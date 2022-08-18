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
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.GetPaymentResponse;

namespace MvcExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITrueLayerClient _truelayer;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITrueLayerClient truelayer, ILogger<HomeController> logger)
        {
            _truelayer = truelayer;
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

            OneOf<Provider.UserSelected, Provider.Preselected> filter = donateModel.UserPreSelectedFilter
                ? new Provider.Preselected("mock-payments-gb-redirect", "faster_payments_service")
                : new Provider.UserSelected();

            var paymentRequest = new CreatePaymentRequest(
                donateModel.AmountInMajor.ToMinorCurrencyUnit(2),
                Currencies.GBP,
                new PaymentMethod.BankTransfer(
                    filter,
                    new Beneficiary.ExternalAccount(
                        "TrueLayer",
                        "truelayer-dotnet",
                        new AccountIdentifier.SortCodeAccountNumber("567890", "12345678"))),
                new PaymentUserRequest(name: donateModel.Name, email: donateModel.Email)
            );

            var apiResponse = await _truelayer.Payments.CreatePayment(
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

            string redirectLink = _truelayer.Payments.CreateHostedPaymentPageLink(
                apiResponse.Data!.Id, apiResponse.Data!.ResourceToken, new Uri(Url.ActionLink("Complete")));

            return Redirect(redirectLink);
        }

        [HttpGet]
        public async Task<IActionResult> Complete([FromQuery(Name = "payment_id")]string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var apiResponse = await _truelayer.Payments.GetPayment(paymentId);

            IActionResult Failed(string status, OneOf<PaymentMethod.BankTransfer>? paymentMethod)
            {
                ViewData["Status"] = status;

                SetProviderAndSchemeId(paymentMethod);
                return View("Failed");
            }

            IActionResult SuccessOrPending(PaymentDetails payment)
            {
                ViewData["Status"] = payment.Status;
                SetProviderAndSchemeId(payment.PaymentMethod);
                return View("Success");
            }

            void SetProviderAndSchemeId(OneOf<PaymentMethod.BankTransfer>? paymentMethod)
            {
                (string providerId, string schemeId) = paymentMethod?.Match(
                    bankTransfer => bankTransfer.ProviderSelection.Match(
                        userSelected => (userSelected.ProviderId, userSelected.SchemeId),
                        preselected => (preselected.ProviderId, preselected.SchemeId)
                    )) ??  ("unavailable", "unavailable");

                ViewData["ProviderId"] = providerId;
                ViewData["SchemeId"] = schemeId;
            }

            if (!apiResponse.IsSuccessful)
                return Failed(apiResponse.StatusCode.ToString(), null!);

            return apiResponse.Data.Match(
                authRequired => Failed(authRequired.Status, authRequired.PaymentMethod),
                authorizing => SuccessOrPending(authorizing),
                authorized => SuccessOrPending(authorized),
                success => SuccessOrPending(success),
                settled => SuccessOrPending(settled),
                failed => Failed(failed.Status, failed.PaymentMethod)
            );
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
}
