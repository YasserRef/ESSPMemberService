using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService.Data;
using ESSPMemberService;

namespace ESSPMemberService.Controllers
{
    public class BRANCH_DESCRIPTIONController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BRANCH_DESCRIPTIONController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BRANCH_DESCRIPTION
        public async Task<IActionResult> Index()
        {
            return View(await _context.V_T_BRANCH_DESCRIPTION.ToListAsync());
        }

        // GET: BRANCH_DESCRIPTION/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_T_BRANCH_DESCRIPTION = await _context.V_T_BRANCH_DESCRIPTION
                .FirstOrDefaultAsync(m => m.F_BRANCHNO == id);
            if (v_T_BRANCH_DESCRIPTION == null)
            {
                return NotFound();
            }

            return View(v_T_BRANCH_DESCRIPTION);
        }

        // GET: BRANCH_DESCRIPTION/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BRANCH_DESCRIPTION/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_BRANCHNO,F_TITTLE,F_ADDRESS,F_GAV_ID,F_CENTER_ID,F_MOBILE,F_TEL1,F_TEL2,F_FAX,F_EMAIL,F_LATITUDE,F_LONGITUDE,WORK_TIME")] V_T_BRANCH_DESCRIPTION v_T_BRANCH_DESCRIPTION)
        {
            if (ModelState.IsValid)
            {
                _context.Add(v_T_BRANCH_DESCRIPTION);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(v_T_BRANCH_DESCRIPTION);
        }

        // GET: BRANCH_DESCRIPTION/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_T_BRANCH_DESCRIPTION = await _context.V_T_BRANCH_DESCRIPTION.FindAsync(id);
            if (v_T_BRANCH_DESCRIPTION == null)
            {
                return NotFound();
            }
            return View(v_T_BRANCH_DESCRIPTION);
        }

        // POST: BRANCH_DESCRIPTION/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("F_BRANCHNO,F_TITTLE,F_ADDRESS,F_GAV_ID,F_CENTER_ID,F_MOBILE,F_TEL1,F_TEL2,F_FAX,F_EMAIL,F_LATITUDE,F_LONGITUDE,WORK_TIME")] V_T_BRANCH_DESCRIPTION v_T_BRANCH_DESCRIPTION)
        {
            if (id != v_T_BRANCH_DESCRIPTION.F_BRANCHNO)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_T_BRANCH_DESCRIPTION);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_T_BRANCH_DESCRIPTIONExists(v_T_BRANCH_DESCRIPTION.F_BRANCHNO))
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
            return View(v_T_BRANCH_DESCRIPTION);
        }

        // GET: BRANCH_DESCRIPTION/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_T_BRANCH_DESCRIPTION = await _context.V_T_BRANCH_DESCRIPTION
                .FirstOrDefaultAsync(m => m.F_BRANCHNO == id);
            if (v_T_BRANCH_DESCRIPTION == null)
            {
                return NotFound();
            }

            return View(v_T_BRANCH_DESCRIPTION);
        }

        // POST: BRANCH_DESCRIPTION/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var v_T_BRANCH_DESCRIPTION = await _context.V_T_BRANCH_DESCRIPTION.FindAsync(id);
            if (v_T_BRANCH_DESCRIPTION != null)
            {
                _context.V_T_BRANCH_DESCRIPTION.Remove(v_T_BRANCH_DESCRIPTION);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_T_BRANCH_DESCRIPTIONExists(decimal id)
        {
            return _context.V_T_BRANCH_DESCRIPTION.Any(e => e.F_BRANCHNO == id);
        }
    }
}
