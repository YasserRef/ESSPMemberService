using ESSPMemberService.Data;
using ESSPMemberService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ESSPMemberService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var results = _context.T_NEWS
                     .Where(e => e.F_ACTIVE == 1)
                     .AsEnumerable()              // bring data to memory
                     .OrderByDescending(e => e.F_CREATED_DATE)
                     .Take(6)
                     .ToList();

            return View(results);
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
