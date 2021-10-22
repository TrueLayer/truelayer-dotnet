using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcExample.Models;
using TrueLayer;
using TrueLayer.Payments.Model;

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

            var paymentRequest = new CreatePaymentRequest(
                (long)Math.Round(donateModel.AmountInMajor * 100, 0),
                Currencies.GBP,
                new PaymentMethod.BankTransfer
                {
                    StatementReference = "Your ref"
                },
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new SchemeIdentifier.SortCodeAccountNumber("567890", "12345678")
                )
            );

            var apiResponse = await _truelayer.Payments.CreatePayment(
                paymentRequest,
                idempotencyKey: Guid.NewGuid().ToString()
            );

            if (!apiResponse.IsSuccessful)
            {
                _logger.LogError("Create TrueLayer payment failed with status code {StatusCode}", apiResponse.StatusCode);
                ModelState.AddModelError("", "Payment failed");
                return View("Index");
            }

            string redirectLink = apiResponse.Data.Match(
                authRequired => _truelayer.Payments.CreateHostedPaymentPageLink(
                    authRequired.Id, authRequired.ResourceToken, new Uri(Url.ActionLink("Complete")))
            );

            return Redirect(redirectLink);
        }

        [HttpGet]
        public async Task<IActionResult> Complete(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return RedirectToAction("Index");

            var apiResponse = await _truelayer.Payments.GetPayment(paymentId);

            IActionResult Failed(string status)
            {
                ViewData["Status"] = status;
                return View("Failed");
            }

            if (!apiResponse.IsSuccessful)
                return Failed(apiResponse.StatusCode.ToString());

            return apiResponse.Data.Match(
                authRequired => Failed(authRequired.Status),
                authorizing => Failed(authorizing.Status), // TODO pending
                authorized => Failed(authorized.Status),
                authFailed => Failed(authFailed.Status),
                success => View("Success"),
                settled => View("Success"),
                failed => Failed(failed.Status)
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
