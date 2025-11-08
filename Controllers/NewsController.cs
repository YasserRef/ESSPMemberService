using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService.Data;
using ESSPMemberService.Models.Tables;

namespace ESSPMemberService.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: T_NEWS
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

        public async Task<IActionResult> AllNews()
        {
            return View(await _context.T_NEWS.Where(e => e.F_ACTIVE == 1).ToListAsync());
        }

        // GET: T_NEWS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_New = await _context.T_NEWS
                .FirstOrDefaultAsync(m => m.F_ID == id);
            if (t_New == null)
            {
                return NotFound();
            }

            return View(t_New);
        }

        // GET: T_NEWS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: T_NEWS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_ID,F_TITLE,F_CONTENT,F_IMAGE_URL,F_CREATED_DATE,F_ACTIVE")] T_NEWS t_New)
        {
            if (ModelState.IsValid)
            {
                _context.Add(t_New);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(t_New);
        }

        // GET: T_NEWS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_New = await _context.T_NEWS.FindAsync(id);
            if (t_New == null)
            {
                return NotFound();
            }
            return View(t_New);
        }

        // POST: T_NEWS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("F_ID,F_TITLE,F_CONTENT,F_IMAGE_URL,F_CREATED_DATE,F_ACTIVE")] T_NEWS t_New)
        {
            if (id != t_New.F_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(t_New);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T_NewExists(t_New.F_ID))
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
            return View(t_New);
        }

        // GET: T_NEWS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_New = await _context.T_NEWS
                .FirstOrDefaultAsync(m => m.F_ID == id);
            if (t_New == null)
            {
                return NotFound();
            }

            return View(t_New);
        }

        // POST: T_NEWS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t_New = await _context.T_NEWS.FindAsync(id);
            if (t_New != null)
            {
                _context.T_NEWS.Remove(t_New);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T_NewExists(int id)
        {
            return _context.T_NEWS.Any(e => e.F_ID == id);
        }
    }
}
