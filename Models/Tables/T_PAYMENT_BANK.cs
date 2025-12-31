using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ESSPMemberService.Models.Tables
{
    public class T_PAYMENT
    {
        [Key]
        public int F_ID { get; set; }
        public int F_ORDER_NO { get; set; }
        public int F_COMPANY_NO { get; set; }
        public int F_ORDER_YEAR { get; set; }
        [Display(Name = "التاريخ")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        public int F_MEMBER { get; set; }
        public decimal F_TOTAL_VALUE { get; set; }
        public int F_REFERENCE_NO { get; set; }
        public int F_PAYMENT { get; set; }
        public int? F_HAFZA_NO { get; set; }
        public int? F_HAFZA_YEAR { get; set; }

        //[Display(Name = "اسم البنك")]
        //public string? F_BANK_NAME { get; set; }
        [Display(Name = " كارت ")]
        public string? F_CARD { get; set; }

        [Display(Name = "المبلغ")]
        public decimal F_PAY_FEES { get; set; }
        [Display(Name = "مبلغ توصيل للبريد")]
        public decimal F_POST_COST { get; set; }
        [Display(Name = "الغرامة ")]
        public int F_PENALTY { get; set; }
        //[Display(Name = "صورة الموقع")]
        //public string F_URL_PIC { get; set; }
    }

    public class T_PAYMENT_MAIN
    {
        public T_PAYMENT_MAIN()
        {
            this.F_ORDER_DATE = DateTime.Now;
            this.F_ORDER_YEAR = DateTime.Now.Year;
            this.F_PENALTY_VALUE = 0;
            this.F_IS_PROCESSED = 0;
            this.F_PAYMENT_TYPE_NO = 1;
            this.F_PAYMENT_METHOD_ID = 1;

            this.PaymentMemCard = null;
        }

        [Key]
        public int F_ID { get; set; }
        [Display(Name = "طريقة التوصيل")]
        public short F_PAYMENT_TYPE_NO { get; set; }
        [Display(Name = "طريقة الدفع")]
        public short F_PAYMENT_METHOD_ID { get; set; }
        public int F_ORDER_NO { get; set; }
        public int F_ORDER_YEAR { get; set; }
        [Display(Name = "التاريخ")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        public int F_MEMBER { get; set; }        

        [Display(Name = "مبلغ التوصيل")]
        public decimal F_PENALTY_VALUE { get; set; }

        [Display(Name = "مبلغ الغرامة")]
        public decimal F_SEND_COST { get; set; }

        [Display(Name = "المبلغ")]
        public decimal F_VALUE { get; set; }

        [Display(Name = "الاجمالى")]
        public decimal F_TOTAL_VALUE { get; set; }
        public int F_PAYMENT { get; set; }

        //[Display(Name = "اسم البنك")]
        //public string? F_BANK_NAME { get; set; }
        // [Display(Name = " رقم العملية ")]
        [Display(Name = " رقم حساب البنك ")]
        public string? F_CARD { get; set; }

        // public Blob F_PAY_IMAGE { get; set; }
        [Display(Name = " صوره الإيصال ")]
        public byte[]? F_PAY_IMAGE { get; set; }

        public int? F_HAFZA_NO { get; set; }
        public int? F_HAFZA_YEAR { get; set; }
        public short? F_IS_PROCESSED { get; set; }	
        //[NotMapped]
        //public List<(int F_YEAR, decimal VALUE)> selectedValues { get; set; }
        [NotMapped]
        public T_PAYMENT_MEM_CARD? PaymentMemCard { get; set; }        
    }
    
    public class T_PAYMENT_CASH
    {
        [Key]
        public int F_ID { get; set; }
        public int F_ORDER_NO { get; set; }
        public int F_COMPANY_NO { get; set; }
        public int F_ORDER_YEAR { get; set; }
        [Display(Name = "التاريخ")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        public int F_MEMBER { get; set; }
        public decimal F_TOTAL_VALUE { get; set; }
        public int F_REFERENCE_NO { get; set; }
        public int F_PAYMENT { get; set; }
      

        //[Display(Name = "اسم البنك")]
        //public string F_BANK_NAME { get; set; }
        //[Display(Name = " كارت ")]
        //public string F_CARD { get; set; }

        [Display(Name = "المبلغ")]
        public decimal F_PAY_FEES { get; set; }
        [Display(Name = "مبلغ توصيل للبريد")]
        public decimal F_POST_COST { get; set; }
        [Display(Name = "الغرامة ")]
        public int F_PENALTY { get; set; }
        //[Display(Name = "صورة الموقع")]
        //public string F_URL_PIC { get; set; }
        public int? F_HAFZA_NO { get; set; }
        public int? F_HAFZA_YEAR { get; set; }
    }


    public class T_PAYMENT_MEM_CARD
    {
        public  T_PAYMENT_MEM_CARD()
        {
            this.F_SEND_DATE = DateTime.Now;
            this.F_IS_SEND = 0;
        }

        [Key]
        public int F_ID { get; set; }
        public int F_BRANCHNO { get; set; }
        [Display(Name = "طريقه التوصيل")]
        public short F_DELIVERY_METHOD { get; set; }

        [Display(Name = "طريقه الدفع")]
        public short F_PAYMENT_METHOD { get; set; }

        [Display(Name = "التاريخ")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]       
        public Nullable<System.DateTime> F_SEND_DATE { get; set; }
        public int F_MEM_NO { get; set; }
        [Display(Name = "تكلفة الإرسال")]
        public decimal? F_SEND_COST { get; set; }

        [Display(Name = "العنوان")]
        public string? F_MEM_ADDRESS { get; set; }
        public short F_IS_SEND { get; set; }
    }


    public class T_PAYMENT_METHOD
    {
        [Key]
        public int F_ID { get; set; }
      
        [Display(Name = "العنوان")]
        public string? F_NAME { get; set; }
        public short F_IS_ACTIVE { get; set; }
    }


    public class PaymentMainDto
    {       
        public int F_ID { get; set; }
        [Display(Name = "طريقة التوصيل")]
        public string? F_DELIVERY_METHOD { get; set; }
        [Display(Name = "طريقة الدفع")]
        public string? F_PAYMENT_METHOD { get; set; }
        [Display(Name = "رقم الايصال")]
        public int F_ORDER_NO { get; set; }
        [Display(Name = " سنة الايصال")]
        public int F_ORDER_YEAR { get; set; }
        [Display(Name = "تاريخ الايصال")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        [Display(Name = "رقم القيد")]
        public int F_MEMBER { get; set; }

        [Display(Name = "اسم العضو")]
        public string F_MEMBER_NAME { get; set; }

        [Display(Name = "مبلغ التوصيل")]
        public decimal F_PENALTY_VALUE { get; set; }

        [Display(Name = "مبلغ الغرامة")]
        public decimal F_SEND_COST { get; set; }

        [Display(Name = "المبلغ")]
        public decimal F_VALUE { get; set; }

        [Display(Name = "الاجمالى")]
        public decimal F_TOTAL_VALUE { get; set; }
        public int F_PAYMENT { get; set; }
     
        //[Display(Name = " صوره الإيصال ")]
        //public byte[]? F_PAY_IMAGE { get; set; }
        public T_PAYMENT_MEM_CARD? PaymentMemCard { get; set; }

    }


    public class V_PAYMENT_MAIN
    {
        public int F_ID { get; set; }
        public short F_PAYMENT_TYPE_NO { get; set; }       
        public short F_PAYMENT_METHOD_ID { get; set; }

        [Display(Name = "طريقة التوصيل")]
        public string? F_DELIVERY_METHOD { get; set; }
        [Display(Name = "طريقة الدفع")]
        public string F_PAYMENT_METHOD { get; set; }
        [Display(Name = "رقم الايصال")]
        public int F_ORDER_NO { get; set; }
        [Display(Name = " سنة الايصال")]
        public int F_ORDER_YEAR { get; set; }
        [Display(Name = "تاريخ الايصال")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        [Display(Name = "رقم القيد")]
        public int F_MEMBER { get; set; }

        [Display(Name = "اسم العضو")]
        public string F_MEMBER_NAME { get; set; }

        [Display(Name = "مبلغ التوصيل")]
        public decimal F_PENALTY_VALUE { get; set; }

        [Display(Name = "مبلغ الغرامة")]
        public decimal F_SEND_COST { get; set; }

        [Display(Name = "المبلغ")]
        public decimal F_VALUE { get; set; }

        [Display(Name = "الاجمالى")]
        public decimal F_TOTAL_VALUE { get; set; }
        public int F_PAYMENT { get; set; }

        [Display(Name = " صوره الإيصال ")]
        public byte[]? F_PAY_IMAGE { get; set; }
        public T_PAYMENT_MEM_CARD? PaymentMemCard { get; set; }

    }


    public class PaymentMainWithDetailsDto
    {
        public int F_ID { get; set; }
        public short F_PAYMENT_TYPE_NO { get; set; }
        public short F_PAYMENT_METHOD_ID { get; set; }

        [Display(Name = "طريقة التوصيل")]
        public string? F_DELIVERY_METHOD { get; set; }
        [Display(Name = "طريقة الدفع")]
        public string? F_PAYMENT_METHOD { get; set; }
        [Display(Name = "رقم الايصال")]
        public int F_ORDER_NO { get; set; }
        [Display(Name = " سنة الايصال")]
        public int F_ORDER_YEAR { get; set; }
        [Display(Name = "تاريخ الايصال")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}")]
        public Nullable<System.DateTime> F_ORDER_DATE { get; set; }
        [Display(Name = "رقم القيد")]
        public int F_MEMBER { get; set; }

        [Display(Name = "اسم العضو")]
        public string F_MEMBER_NAME { get; set; }

        [Display(Name = "مبلغ التوصيل")]
        public decimal F_PENALTY_VALUE { get; set; }

        [Display(Name = "مبلغ الغرامة")]
        public decimal F_SEND_COST { get; set; }

        [Display(Name = "المبلغ")]
        public decimal F_VALUE { get; set; }

        [Display(Name = "الاجمالى")]
        public decimal F_TOTAL_VALUE { get; set; }
        public int F_PAYMENT { get; set; }

        [Display(Name = " صوره الإيصال ")]
        public byte[]? F_PAY_IMAGE { get; set; }       
        [Display(Name = "تفاصيل الدفع")]
        public List<PaymentDetailDto>? PaymentDetails { get; set; }
    }



}
