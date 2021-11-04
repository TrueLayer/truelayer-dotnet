using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrueLayer;
using System.Threading.Tasks;

namespace MvcExample.Controllers
{
    public class MerchantsController : Controller
    {
        private readonly ITrueLayerClient _truelayer;
        private readonly ILogger<MerchantsController> _logger;

        public MerchantsController(ITrueLayerClient truelayer, ILogger<MerchantsController> logger)
        {
            _truelayer = truelayer;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var apiResponse = await _truelayer.Merchants.ListMerchants();

            if (apiResponse.IsSuccessful)
            {
                return View(apiResponse.Data?.Items);
            }

            _logger.LogError("Get merchant accounts failed with status code {StatusCode}", apiResponse.StatusCode);

            if (apiResponse.Problem != null)
            {
                var problem = apiResponse.Problem;
                ModelState.AddModelError("", $"{problem.Title}, {problem.Detail}");
            }
            else
            {
                ModelState.AddModelError("", "Failed to gather merchants list");
            }

            return View();

        }
    }
}
