using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class Callback : PageModel
    {
        public string PaymentId;
        
        public void OnGet([FromQuery] string payment_id)
        {
            PaymentId = payment_id;
        }
    }
}
