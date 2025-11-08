using System.ComponentModel.DataAnnotations;

namespace ESSPMemberService.Models.Tables
{
    public class T_PAYMENT_COMPANY
    {
        [Key]
        public int F_CODE { get; set; }
        public string? F_NAME { get; set; }
        public decimal? F_PAY_FEES { get; set; }
    }
}
