using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class MaterialUpdateModel
    {
        [Required]
        public string Code { get; set; } // Unique ID
        [Required]
        public string Description { get; set; }
        [Required]
        public string MaterialGroupCode { get; set; }
    }
}
