using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class CustomerMaterialListingUpdateModel
    {
        [Required]
        public string CustomerCode { get; set; } //Unique ID
        [Required]
        public string MaterialCode { get; set; }
    }
}
