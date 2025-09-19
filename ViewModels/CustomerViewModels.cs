using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class CustomerUpdateModel
    {
        [Required]
        public string Code { get; set; } //Unique Id
        [Required]
        public string Name { get; set; }
        [Required]
        public string AllocationBlock { get; set; }
        [Required]
        public string DeliveryBlock { get; set; }
    }
}
