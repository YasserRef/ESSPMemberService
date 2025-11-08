using System.ComponentModel.DataAnnotations;

namespace ESSPMemberService.Models.Tables
{
    public partial class T_PAYMENT_DETAIL
    {
        [Key]
        public decimal F_ID { get; set; }
        public int F_MAIN_ID { get; set; }
        //[Display(Name = "رقم شركة الدفع")]
        //public int F_COMPANY_NO { get; set; }
        //public int F_ORDER_YEAR { get; set; }
        //[Display(Name = "التاريخ")]
        //[DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        //public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        public int F_SER { get; set; }
        public decimal? F_DESC_PAY { get; set; }
        public int F_PAY_YEAR { get; set; }
        public int F_PAY_MONTH { get; set; }
        public int F_PAY_VALUE { get; set; }
        public int F_ISHTRAKAT { get; set; }
        public int F_SERVICE_BOX { get; set; }

        [Display(Name = "كارت")]
        public string? F_ESSP_CARD { get; set; }
        [Display(Name = "رسوم النقابة")]
        public string? F_ESSP_FEES { get; set; }         
    }

    public partial class PaymentDetailDto
    {
        [Key]
       
        public int F_SER { get; set; }
        public decimal? F_DESC_PAY { get; set; }
        public int F_PAY_YEAR { get; set; }
        // public int F_PAY_MONTH { get; set; }
        public int F_PAY_VALUE { get; set; }
        //public int F_ISHTRAKAT { get; set; }
        //public int F_SERVICE_BOX { get; set; }

        //[Display(Name = "كارت")]
        //public string? F_ESSP_CARD { get; set; }
        //[Display(Name = "رسوم النقابة")]
        //public string? F_ESSP_FEES { get; set; }
    }
}
