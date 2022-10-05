using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrueLayer;
using TrueLayer.PaymentsProviders.Model;

namespace MvcExample.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly ITrueLayerClient _truelayer;

        public ProvidersController(ITrueLayerClient truelayer)
        {
            _truelayer = truelayer;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetProvider([FromQuery(Name = "id")] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            var apiResponse = await _truelayer.PaymentsProviders.GetPaymentsProvider(providerId);

            IActionResult Failed(string status)
            {
                ViewData["Status"] = status;
                ViewData["ProviderId"] = providerId;

                return View("Failed");
            }

            IActionResult Success(PaymentsProvider provider)
            {
                ViewData["ProviderId"] = provider.Id;
                ViewData["LogoUri"] = provider.LogoUri;
                return View("Success");
            }

            return apiResponse.IsSuccessful
                ? Success(apiResponse.Data)
                : Failed(apiResponse.StatusCode.ToString());
        }
    }
}
