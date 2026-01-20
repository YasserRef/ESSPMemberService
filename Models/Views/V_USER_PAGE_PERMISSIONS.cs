using System.ComponentModel.DataAnnotations;

namespace ESSPMemberService.Models.Views
{

    public class T_USERPERMISSIONS
    {
        [Key]
        public int F_ID { get; set; }

        [Display(Name = "رقم المستخدم")]
        public int F_USER_ID { get; set; }
        public int F_PAGE_ID { get; set; }

        // 🔑 Navigation property
        public virtual T_PAGE_PERMISSIONS T_PAGE_PERMISSIONS { get; set; }
    }

    public class T_PAGE_PERMISSIONS
    {
        [Key]       
        public int F_PAGE_ID { get; set; }

        [Display(Name = "الصلاحيات")]
        public required string F_PAGE_NAME { get; set; }
    }

    public class V_USER_PAGE_PERMISSIONS
    {
        //[Key]
        public int F_ID { get; set; }

        [Display(Name = "رقم المستخدم")]
        public int F_USER_ID { get; set; }
        public int F_PAGE_ID { get; set; }

        [Display(Name = "الصلاحيات")]
        public required string F_PAGE_NAME { get; set; }
    }
}
