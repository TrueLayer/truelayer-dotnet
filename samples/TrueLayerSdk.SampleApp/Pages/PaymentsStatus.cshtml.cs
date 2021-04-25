using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrueLayerSdk.SampleApp.Data;

namespace TrueLayerSdk.SampleApp.Pages
{
    public class PaymentsStatus : PageModel
    {
        private readonly PaymentsDbContext _context;
        public List<PaymentEntity> Payments { get; private set; }

        public PaymentsStatus(PaymentsDbContext context)
        {
            _context = context;
        }
        
        public async Task OnGet()
        {
            Payments = await _context.Payments.ToListAsync();
        }
        
        public IActionResult OnGetStatus(string paymentId)
        {
            return RedirectToPage("Callback", new {payment_id = paymentId});
        }
    }
}
