using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class CustomerRouteListModel
    {
        [Required]
        public string? RouteCode { get; set; }
        [Required]
        public string? CustomerCode { get; set; }
    }
}
