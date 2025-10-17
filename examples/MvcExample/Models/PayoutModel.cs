using System.ComponentModel.DataAnnotations;

namespace MvcExample.Models;

public class PayoutModel
{
    [Required(ErrorMessage = "The amount is required")]
    [Range(1, double.MaxValue, ErrorMessage = "You must pay at least 1 GBP")]
    public decimal AmountInMajor { get; set; }

    [Required(ErrorMessage = "Your merchant account Id is required")]
    public string MerchantAccountId { get; set; }

    [Required(ErrorMessage = "Your IBAN is required")]
    public string Iban { get; set; }
}