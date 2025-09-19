using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class CustomerRouteListingUpdateModel
    {
        [Required]
        public string CustomerCode { get; set; } // Unique ID
        [Required]
        public string RouteCode { get; set; }
    }
}
