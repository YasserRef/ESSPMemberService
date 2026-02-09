namespace ESSPMemberService.Models
{
    public class AppSettings
    {
    }

    public class HomePageOptions
    {
        public List<Slide> Slides { get; set; }
        public Banner BannerRight { get; set; }
        public Banner BannerLeft { get; set; }
    }

    public class Slide
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Banner
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

}
