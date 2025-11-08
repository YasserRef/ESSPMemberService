using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService;
using ESSPMemberService.Data;
using Oracle.ManagedDataAccess.Client;

namespace ESSPMemberService.Controllers
{
    public class V_SPENDDATEController : Controller
    {
        private readonly ApplicationDbContext _context;

        public V_SPENDDATEController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: V_SPENDDATE
        public async Task<IActionResult> Index()
        {
            long MemId = Convert.ToInt64(HttpContext.Session.GetString("MemID"));
            if (MemId <= 0)
                return RedirectToAction("Index", "V_MEMBER_INFO");

            // var result = _context.V_SPENDDATE.Where(e => e.F_MEMBER == MemId).ToList();

            var result = await _context.V_SPENDDATE
            .FromSqlRaw("SELECT DISTINCT * FROM V_SPENDDATE WHERE F_MEMBER = :MemId AND ROWNUM <= 2", new OracleParameter("MemId", MemId))
            .ToListAsync();

            return View(result);
        }


        // GET: V_SPENDDATE
        public async Task<IActionResult> IndexSelf()
        {
            long MemId = Convert.ToInt64(HttpContext.Session.GetString("MemID"));
            if (MemId <= 0)
                return RedirectToAction("Index", "V_MEMBER_INFO");

            var result = await _context.V_SPENDDATE_SELF
            .FromSqlRaw("SELECT * FROM V_SPENDDATE_SELF WHERE F_MEMBER = :MemId AND ROWNUM <= 2", new OracleParameter("MemId", MemId))
            .ToListAsync();

            return View(result);
        }

        // GET: V_SPENDDATE/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_SPENDDATE = await _context.V_SPENDDATE
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (v_SPENDDATE == null)
            {
                return NotFound();
            }

            return View(v_SPENDDATE);
        }

        // GET: V_SPENDDATE/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: V_SPENDDATE/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_MEMBER,F_CODE,F_VALUE,F_PERSON,F_ACCOUNTNO,GIRO_NO,M_BANK,S_BANK,F_SPENDWAY,SPENDWAY,MEM_NAME,F_SPENDATE,F_SPENDATE_OUT,F_DOFA,F_YEAR")] V_SPENDDATE v_SPENDDATE)
        {
            if (ModelState.IsValid)
            {
                _context.Add(v_SPENDDATE);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(v_SPENDDATE);
        }

        // GET: V_SPENDDATE/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_SPENDDATE = await _context.V_SPENDDATE.FindAsync(id);
            if (v_SPENDDATE == null)
            {
                return NotFound();
            }
            return View(v_SPENDDATE);
        }

        // POST: V_SPENDDATE/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("F_MEMBER,F_CODE,F_VALUE,F_PERSON,F_ACCOUNTNO,GIRO_NO,M_BANK,S_BANK,F_SPENDWAY,SPENDWAY,MEM_NAME,F_SPENDATE,F_SPENDATE_OUT,F_DOFA,F_YEAR")] V_SPENDDATE v_SPENDDATE)
        {
            if (id != v_SPENDDATE.F_CODE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_SPENDDATE);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_SPENDDATEExists(v_SPENDDATE.F_CODE))
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
            return View(v_SPENDDATE);
        }

        // GET: V_SPENDDATE/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_SPENDDATE = await _context.V_SPENDDATE
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (v_SPENDDATE == null)
            {
                return NotFound();
            }

            return View(v_SPENDDATE);
        }

        // POST: V_SPENDDATE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var v_SPENDDATE = await _context.V_SPENDDATE.FindAsync(id);
            if (v_SPENDDATE != null)
            {
                _context.V_SPENDDATE.Remove(v_SPENDDATE);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_SPENDDATEExists(decimal id)
        {
            return _context.V_SPENDDATE.Any(e => e.F_CODE == id);
        }
    }
}
