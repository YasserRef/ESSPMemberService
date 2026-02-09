using ESSPMemberService.Models.Tables;

namespace ESSPMemberService.Models
{
    public class HomeViewModel
    {
        public IEnumerable<T_NEWS> News { get; set; }
        public HomePageOptions HomePage { get; set; }
    }

}
