using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService.Data;
using ESSPMemberService.Models.Tables;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json.Linq;

// using Oracle.EntityFrameworkCore;

namespace ESSPMemberService.Controllers
{
    public class T_PAYMENT_COMPANYController : Controller
    {
        private readonly ApplicationDbContext _context;

        public T_PAYMENT_COMPANYController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: T_PAYMENT_COMPANY
        public async Task<IActionResult> Index()
        {
            return View(await _context.T_PAYMENT_COMPANY.ToListAsync());
        }

        // GET: T_PAYMENT_COMPANY/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_PAYMENT_COMPANY = _context.T_PAYMENT_COMPANY.Where(e => e.F_CODE == id).Single();
               // .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (t_PAYMENT_COMPANY == null)
            {
                return NotFound();
            }

            return View(t_PAYMENT_COMPANY);
        }

        // GET: T_PAYMENT_COMPANY/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: T_PAYMENT_COMPANY/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_CODE,F_NAME,F_PAY_FEES")] T_PAYMENT_COMPANY t_PAYMENT_COMPANY)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //                var result = _context.Database.ExecuteSqlCommand("BEGIN your_procedure_name(:param1, :param2); END;",
                    //new OracleParameter("param1", value1),
                    //new OracleParameter("param2", value2));

                    var sql = "INSERT INTO T_PAYMENT_COMPANY (F_CODE,F_NAME, F_PAY_FEES) VALUES (:param1, :param2, :param3)";
                    await _context.Database.ExecuteSqlRawAsync(sql, new OracleParameter("param1", 4), new OracleParameter("param2", t_PAYMENT_COMPANY.F_NAME), new OracleParameter("param3", t_PAYMENT_COMPANY.F_PAY_FEES));

                    _context.Add(t_PAYMENT_COMPANY);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(t_PAYMENT_COMPANY);
            }
            catch (DbUpdateException ex)
            {
                // Log the inner exception for more details
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }
        }

        // GET: T_PAYMENT_COMPANY/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var t_PAYMENT_COMPANY = await _context.T_PAYMENT_COMPANY.FindAsync(id);

            var t_PAYMENT_COMPANY = await _context.T_PAYMENT_COMPANY
                                      .Where(e => e.F_CODE == id).ToListAsync();
                                      
            if (t_PAYMENT_COMPANY[0] == null)
            {
                return NotFound();
            }
            return View(t_PAYMENT_COMPANY[0]);
        }

        // POST: T_PAYMENT_COMPANY/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("F_CODE,F_NAME,F_PAY_FEES")] T_PAYMENT_COMPANY t_PAYMENT_COMPANY)
        {
            if (id != t_PAYMENT_COMPANY.F_CODE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   // _context.Update(t_PAYMENT_COMPANY);

                    // Example of using raw SQL
                    var sql = "Update T_PAYMENT_COMPANY Set (F_NAME:param2, F_PAY_FEES:param3) Where t_PAYMENT_COMPANY.F_CODE = :param1";
                    await _context.Database.ExecuteSqlRawAsync(sql, new OracleParameter("param1",4 ), new OracleParameter("param2", t_PAYMENT_COMPANY.F_NAME), new OracleParameter("param3", t_PAYMENT_COMPANY.F_PAY_FEES));

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T_PAYMENT_COMPANYExists(t_PAYMENT_COMPANY.F_CODE))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (OracleException ex)
                {
                    Console.WriteLine($"Oracle Error: {ex.Message}");
                    // Handle specific Oracle exceptions here
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Entity Framework Error: {ex.InnerException?.Message}");
                    // Handle general EF errors here
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    // Handle other exceptions here
                }

                return RedirectToAction(nameof(Index));
            }
            return View(t_PAYMENT_COMPANY);
        }

        // GET: T_PAYMENT_COMPANY/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_PAYMENT_COMPANY = await _context.T_PAYMENT_COMPANY
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (t_PAYMENT_COMPANY == null)
            {
                return NotFound();
            }

            return View(t_PAYMENT_COMPANY);
        }

        // POST: T_PAYMENT_COMPANY/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t_PAYMENT_COMPANY = await _context.T_PAYMENT_COMPANY.FindAsync(id);
            if (t_PAYMENT_COMPANY != null)
            {
                var sql = "Delete From T_PAYMENT_COMPANY Where t_PAYMENT_COMPANY.F_CODE = :param1";
                await _context.Database.ExecuteSqlRawAsync(sql, new OracleParameter("param1", id));

               // _context.T_PAYMENT_COMPANY.Remove(t_PAYMENT_COMPANY);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T_PAYMENT_COMPANYExists(int id)
        {
            return _context.T_PAYMENT_COMPANY.Any(e => e.F_CODE == id);
        }
    }
}
