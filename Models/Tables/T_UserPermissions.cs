using System.ComponentModel.DataAnnotations;

namespace ESSPMemberService.Models.Tables
{
    public class T_USERPERMISSIONS
    {
        [Key]
      
        [Display(Name = "رقم المستخدم")]
        public int F_UserId { get; set; }

        [Display(Name = "الصلاحيات")]
        public string? F_PermissionCode { get; set; }
    }
}
