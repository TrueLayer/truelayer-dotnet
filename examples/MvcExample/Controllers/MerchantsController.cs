using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrueLayer;
using System.Linq;
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

            if (!apiResponse.IsSuccessful)
            {
                if (apiResponse.Problem?.Errors != null)
                {
                    foreach (var error in apiResponse.Problem.Errors)
                    {
                        ModelState.AddModelError("", $"{error.Key}: {error.Value?.FirstOrDefault()}");
                    }
                }
                
                ModelState.AddModelError("", "Failed to gather merchants list");
                return View();
            }
            
            return View(apiResponse.Data?.Items);
        }
    }
}
