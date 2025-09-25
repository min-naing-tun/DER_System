using System.ComponentModel.DataAnnotations;

namespace DER_System.ViewModels
{
    public class MaterialModel
    {
        [Required]
        public string? Code { get; set; } // Unique ID
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? MaterialType { get; set; }
        public string Active { get; set; } = "";
    }
}
