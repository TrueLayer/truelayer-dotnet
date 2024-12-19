using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrueLayer;

namespace MvcExample.Controllers
{
    public class MerchantAccountsController : Controller
    {
        private readonly ITrueLayerClient _trueLayerClient;
        private readonly ILogger<MerchantAccountsController> _logger;

        public MerchantAccountsController([FromKeyedServices("TrueLayerClient")]ITrueLayerClient trueLayerClient, ILogger<MerchantAccountsController> logger)
        {
            _trueLayerClient = trueLayerClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var apiResponse = await _trueLayerClient.MerchantAccounts.ListMerchantAccounts();

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

        public async Task<IActionResult> Details(string id)
        {
            var apiResponse = await _trueLayerClient.MerchantAccounts.GetMerchantAccount(id);

            if (apiResponse.IsSuccessful)
            {
                return View(apiResponse.Data);
            }

            _logger.LogError("Get merchant details failed with status code {StatusCode}", apiResponse.StatusCode);

            if (apiResponse.Problem != null)
            {
                var problem = apiResponse.Problem;
                ModelState.AddModelError("", $"{problem.Title}, {problem.Detail}");
            }
            else
            {
                ModelState.AddModelError("", "Failed to gather merchant details");
            }

            return View();
        }
    }
}
