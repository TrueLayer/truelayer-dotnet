using System.ComponentModel.DataAnnotations;

namespace MvcExample.Models
{
    public class DonateModel
    {
        [Required(ErrorMessage = "The amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must donate at least 1 GBP")]
        public decimal AmountInMajor { get; set; }

        [Required(ErrorMessage = "Your name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Your email is required")]
        [EmailAddress(ErrorMessage = "A valid email address is required")]
        public string Email { get; set; }

        public bool UserPreSelectedFilter { get; set; }
    }
}
