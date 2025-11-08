using ESSPMemberService.Models.Tables;

namespace ESSPMemberService
{
    internal class YourViewModel
    {
        public required T_PAYMENT_MAIN Model { get; set; }
        public List<(int F_YEAR, decimal VALUE)>? SelectedValues { get; set; }
    }
}