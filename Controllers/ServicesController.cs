using Microsoft.AspNetCore.Mvc;

namespace ESSPMemberService.Controllers
{
    public class ServicesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Eshtark()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult EstsharyDoc()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult MasarifGanazaDoc()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SpendDateDoc()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SpendDateSelfDoc()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}