namespace MvcExample.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DonateModel
    {
        [Required(ErrorMessage = "The amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must donate at least 1 GBP")]
        public double AmountInMajor { get; set; }
    }
}
