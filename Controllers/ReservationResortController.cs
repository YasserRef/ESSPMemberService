using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService;
using ESSPMemberService.Data;

namespace ESSPMemberService.Controllers
{
    public class ReservationResortController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationResortController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: V_RESERVATION_RESORT

        public async Task<IActionResult> Index(int? branchNo, int? holidayCode, int? weekNo, int? flatCode)
        {
            var query = _context.V_RESERVATION_RESORT
                    .Where(static r => r.HOLIDAY_YEAR == DateTime.Now.Year) //  && r.F_MEM == null
                    .AsQueryable();

            if (branchNo.HasValue)
                query = query.Where(r => r.BRANCH_NO == branchNo);

            if (holidayCode.HasValue)
                query = query.Where(r => r.HOLIDAY_CODE == holidayCode);

            if (weekNo.HasValue)
                query = query.Where(r => r.WEEK_NO == weekNo);

            if (flatCode.HasValue)
                query = query.Where(r => r.FLAT_CODE == flatCode);

            var results = await query.ToListAsync();
            return View(results);
        }

        public async Task<IActionResult> Index1()
        {
            var results = await _context.V_RESERVATION_RESORT
                    .Where(static r => r.HOLIDAY_YEAR == DateTime.Now.Year) //  && r.F_MEM == null
                    .ToListAsync();
            return View(results);
        }

        // GET: V_RESERVATION_RESORT/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_RESERVATION_RESORT = await _context.V_RESERVATION_RESORT
                .FirstOrDefaultAsync(m => m.RECIEPT_NO == id);
            if (v_RESERVATION_RESORT == null)
            {
                return NotFound();
            }

            return View(v_RESERVATION_RESORT);
        }

        // GET: V_RESERVATION_RESORT/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: V_RESERVATION_RESORT/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RECIEPT_NO,RECIEPT_YEAR,RECIEPT_DATE,BRANCH_NO,USER_CODE,F_MEM,HOLIDAY_CODE,HOLIDAY_YEAR,FLAT_CODE,WEEK_NO,FLAT_VALUE,INSURE_VALUE,RECIEPT_VALUE,CANCEL,DISCOUNT,F_STAMP,RES_PK")] V_RESERVATION_RESORT v_RESERVATION_RESORT)
        {
            if (ModelState.IsValid)
            {
                _context.Add(v_RESERVATION_RESORT);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(v_RESERVATION_RESORT);
        }

        // GET: V_RESERVATION_RESORT/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_RESERVATION_RESORT = await _context.V_RESERVATION_RESORT.FindAsync(id);
            if (v_RESERVATION_RESORT == null)
            {
                return NotFound();
            }
            return View(v_RESERVATION_RESORT);
        }

        // POST: V_RESERVATION_RESORT/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("RECIEPT_NO,RECIEPT_YEAR,RECIEPT_DATE,BRANCH_NO,USER_CODE,F_MEM,HOLIDAY_CODE,HOLIDAY_YEAR,FLAT_CODE,WEEK_NO,FLAT_VALUE,INSURE_VALUE,RECIEPT_VALUE,CANCEL,DISCOUNT,F_STAMP,RES_PK")] V_RESERVATION_RESORT v_RESERVATION_RESORT)
        {
            if (id != v_RESERVATION_RESORT.RECIEPT_NO)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_RESERVATION_RESORT);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_RESERVATION_RESORTExists(v_RESERVATION_RESORT.RECIEPT_NO))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(v_RESERVATION_RESORT);
        }

        // GET: V_RESERVATION_RESORT/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_RESERVATION_RESORT = await _context.V_RESERVATION_RESORT
                .FirstOrDefaultAsync(m => m.RECIEPT_NO == id);
            if (v_RESERVATION_RESORT == null)
            {
                return NotFound();
            }

            return View(v_RESERVATION_RESORT);
        }

        // POST: V_RESERVATION_RESORT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var v_RESERVATION_RESORT = await _context.V_RESERVATION_RESORT.FindAsync(id);
            if (v_RESERVATION_RESORT != null)
            {
                _context.V_RESERVATION_RESORT.Remove(v_RESERVATION_RESORT);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_RESERVATION_RESORTExists(decimal id)
        {
            return _context.V_RESERVATION_RESORT.Any(e => e.RECIEPT_NO == id);
        }
    }
}
