using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class CustomerModel
    {
        [Required]
        public string? Code { get; set; } //Unique Id
        [Required]
        public string? Description { get; set; }
        public string AllocationBlock { get; set; } = "";
        public string CentralBlock { get; set; } = "";
        public string Active { get; set; } = "";
    }
}
