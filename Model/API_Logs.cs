using System.ComponentModel.DataAnnotations;

namespace DER_System.Model
{
    public class API_Logs
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; } //request or response
        public string Endpoint { get; set; }
        public string Data { get; set; }
        public DateTime ActionTime { get; set; }
    }
}
