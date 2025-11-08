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
    public class TRAINING_DESCRIPTIONController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TRAINING_DESCRIPTIONController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: V_T_TRAINING_DESCRIPTION
        public async Task<IActionResult> Index()
        {
            return View(await _context.V_T_TRAINING_DESCRIPTION.ToListAsync());
        }

        // GET: V_T_TRAINING_DESCRIPTION/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRAINING_DESCRIPTION = await _context.V_T_TRAINING_DESCRIPTION
                .Where(m => m.F_CODE == id).ToListAsync();
            if (tRAINING_DESCRIPTION == null)
            {
                return NotFound();
            }

            return View(tRAINING_DESCRIPTION[0]);
        }

        // GET: V_T_TRAINING_DESCRIPTION/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: V_T_TRAINING_DESCRIPTION/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_CODE,F_TRANING_DES,F_TRAINER_NAME,F_TRAINER_PHONE,F_ADDRESS,F_DATE,F_VALUE,F_DISCOUNT,F_URL_PIC")] V_T_TRAINING_DESCRIPTION tRAINING_DESCRIPTION)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tRAINING_DESCRIPTION);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tRAINING_DESCRIPTION);
        }

        // GET: V_T_TRAINING_DESCRIPTION/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRAINING_DESCRIPTION = await _context.V_T_TRAINING_DESCRIPTION.FindAsync(id);
            if (tRAINING_DESCRIPTION == null)
            {
                return NotFound();
            }
            return View(tRAINING_DESCRIPTION);
        }

        // POST: V_T_TRAINING_DESCRIPTION/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("F_CODE,F_TRANING_DES,F_TRAINER_NAME,F_TRAINER_PHONE,F_ADDRESS,F_DATE,F_VALUE,F_DISCOUNT,F_URL_PIC")] V_T_TRAINING_DESCRIPTION tRAINING_DESCRIPTION)
        {
            if (id != tRAINING_DESCRIPTION.F_CODE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tRAINING_DESCRIPTION);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TRAINING_DESCRIPTIONExists(tRAINING_DESCRIPTION.F_CODE))
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
            return View(tRAINING_DESCRIPTION);
        }

        // GET: V_T_TRAINING_DESCRIPTION/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tRAINING_DESCRIPTION = await _context.V_T_TRAINING_DESCRIPTION
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (tRAINING_DESCRIPTION == null)
            {
                return NotFound();
            }

            return View(tRAINING_DESCRIPTION);
        }

        // POST: V_T_TRAINING_DESCRIPTION/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tRAINING_DESCRIPTION = await _context.V_T_TRAINING_DESCRIPTION.FindAsync(id);
            if (tRAINING_DESCRIPTION != null)
            {
                _context.V_T_TRAINING_DESCRIPTION.Remove(tRAINING_DESCRIPTION);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TRAINING_DESCRIPTIONExists(int id)
        {
            return _context.V_T_TRAINING_DESCRIPTION.Any(e => e.F_CODE == id);
        }
    }
}
