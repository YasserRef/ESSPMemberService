using System.ComponentModel.DataAnnotations;

namespace ESSPMemberService.Models.Tables
{
    public class T_NEWS
    {
        [Key]
        public int F_ID { get; set; }
        public string? F_NAME { get; set; }
        public string? F_TITLE { get; set; }
        public string? F_CONTENT { get; set; }
        public string? F_IMAGE_URL { get; set; }
        public DateTime? F_CREATED_DATE { get; set; }
        public short F_ACTIVE { get; set; }	

    }
}
