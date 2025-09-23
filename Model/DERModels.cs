using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DER_System.Model
{
    public class Users
    {
        [Key]
        public Guid? SysKey { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64? AutoID { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Guid? UserRoleSysKey { get; set; }
        public short? UserRoleNo { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
    }

    public class Customers
    {
        [Key]
        public Guid? SysKey { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 AutoID { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public short AlloBlock { get; set; }
        public short CentralBlock { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
    }
}
