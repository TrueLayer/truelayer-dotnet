
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcExample.Models;
using TrueLayer;
using TrueLayer.Payouts.Model;
using static TrueLayer.Payouts.Model.GetPayoutsResponse;

namespace MvcExample.Controllers
{
    public class PayoutController : Controller
    {
        private readonly ITrueLayerClient _truelayer;
        private readonly ILogger<PayoutController> _logger;

        public PayoutController(ITrueLayerClient truelayer, ILogger<PayoutController> logger)
        {
            _truelayer = truelayer;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayout(PayoutModel payoutModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            var payoutRequest = new CreatePayoutRequest(
                payoutModel.MerchantAccountId,
                payoutModel.AmountInMajor.ToMinorCurrencyUnit(2),
                Currencies.GBP,
                new Beneficiary.ExternalAccount(
                    "TrueLayer",
                    "truelayer-dotnet",
                    new AccountIdentifier.Iban(payoutModel.Iban)
                )
            );

            var apiResponse = await _truelayer.Payouts.CreatePayout(
                payoutRequest,
                idempotencyKey: Guid.NewGuid().ToString()
            );

            if (!apiResponse.IsSuccessful)
            {
                _logger.LogError("Create TrueLayer payout failed with status code {StatusCode}", apiResponse.StatusCode);

                if (apiResponse.Problem?.Errors != null)
                {
                    foreach (var error in apiResponse.Problem.Errors)
                    {
                        ModelState.AddModelError("", $"{error.Key}: {error.Value?.FirstOrDefault()}");
                    }
                }

                ModelState.AddModelError("", "Payout failed");
                return View("Index");
            }

            var redirectLink = new Uri(string.Join(
                "/",
                Url.ActionLink("Complete").TrimEnd('/'),
                $"?payoutId={apiResponse.Data?.Id}"));

            return Redirect(redirectLink.AbsoluteUri);
        }

        [HttpGet]
        public async Task<IActionResult> Complete(string payoutId)
        {
            if (string.IsNullOrWhiteSpace(payoutId))
            {
                return View();
            }

            var apiResponse = await _truelayer.Payouts.GetPayout(payoutId);

            IActionResult Failed(string status)
            {
                ViewData["Status"] = status;
                return View("Failed");
            }

            IActionResult Success(PayoutDetails payout)
            {
                ViewData["Status"] = payout.Status;
                return View("Success");
            }

            IActionResult Pending(PayoutDetails payout)
            {
                ViewData["Status"] = payout.Status;
                return View("Success");
            }

            if (!apiResponse.IsSuccessful)
                return Failed(apiResponse.StatusCode.ToString());

            return apiResponse.Data.Match(
                authorizing => Pending(authorizing),
                authorized => Success(authorized),
                executed => Success(executed),
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
