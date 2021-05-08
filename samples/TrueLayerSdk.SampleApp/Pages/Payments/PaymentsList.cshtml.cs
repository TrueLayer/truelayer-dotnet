using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrueLayerSdk.SampleApp.Data;
using System.Linq;

namespace TrueLayerSdk.SampleApp.Pages.Payments
{
    public class PaymentsList : PageModel
    {
        private readonly PaymentsDbContext _context;
        public List<PaymentEntity> Payments { get; private set; }

        public PaymentsList(PaymentsDbContext context)
        {
            _context = context;
        }
        
        public async Task OnGet()
        {
            Payments = await _context.Payments.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
        
        public IActionResult OnGetStatus(string paymentId)
        {
            return RedirectToPage("/Callback", new {single_immediate_payment_id = paymentId});
        }
    }
}
