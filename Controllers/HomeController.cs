using ESSPMemberService.Data;
using ESSPMemberService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ESSPMemberService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<HomePageOptions> _homePageOptions;


        public HomeController(ApplicationDbContext context, IOptions<HomePageOptions> homePageOptions)
        {
            _context = context;
            _homePageOptions = homePageOptions;
        }

        public async Task<IActionResult> Index()
        {
            var news = _context.T_NEWS
                     .Where(e => e.F_ACTIVE == 1)
                     .AsEnumerable()              // bring data to memory
                     .OrderByDescending(e => e.F_CREATED_DATE)
                     .Take(6)
                     .ToList();

            var model = new HomeViewModel
            {
                News = news,
                HomePage = _homePageOptions.Value
            };

            return View(model);
        }

        public IActionResult Admin()
        {
            return View();
        }        

        public IActionResult WhoWe()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GooglereCAPTCHA()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public IActionResult Print()
        {
            return View();
        }



    }
}
