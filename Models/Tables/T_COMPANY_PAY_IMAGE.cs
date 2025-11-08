using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ESSPMemberService.Models.Tables
{
    public partial class T_COMPANY_PAY_IMAGE
    {
        [Key]
        public int F_ORDER_NO { get; set; }
        public int F_COMPANY_NO { get; set; }
        public int F_ORDER_YEAR { get; set; }

        [Display(Name = "صورة الموقع")]
        public Blob F_PAY_IMAGE { get; set; }
    }
}
