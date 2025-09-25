using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class CustomerMaterialListingModel
    {
        [Required]
        public string? CustomerCode { get; set; }
        [Required]
        public string? MaterialCode { get; set; }
        public string Active { get; set; } = "";
    }
}
