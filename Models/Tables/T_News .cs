using System.ComponentModel.DataAnnotations;

namespace ESSPMemberService.Models.Tables
{
    public class T_NEWS
    {
        [Key]
        public int F_ID { get; set; }
        [Display(Name = "اسم الكاتب")]
        public string? F_NAME { get; set; }
        [Display(Name = "عنوان الخبر")]
        public string? F_TITLE { get; set; }
        [Display(Name = "نص الخبر")]
        public string? F_CONTENT { get; set; }
        [Display(Name = "الصورة ")]
        public string? F_IMAGE_URL { get; set; }
        [Display(Name = "التاريخ")]
        public DateTime? F_CREATED_DATE { get; set; }
        [Display(Name = "الحالة")]
        public short F_ACTIVE { get; set; }	

    }
}
