using ESSPMemberService;
using ESSPMemberService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ESSPMemberService.Controllers
{
    public class ResortsNotReservedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResortsNotReservedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: V_RESORT_NOT_RESERVED
        public IActionResult Index(string holidayName, int? weekNo, string flatDesc)
        {
            var resorts = _context.V_RESORT_NOT_RESERVED.AsQueryable();

            // Get distinct lists
            var holidayNames = resorts.Select(x => x.HOLDAY_NAME).Distinct().OrderBy(n => n).ToList();
            var weekNos = string.IsNullOrWhiteSpace(holidayName)
                            ? resorts.Select(x => x.F_WEEK).Distinct().OrderBy(n => n).ToList()
                            : resorts.Where(r => r.HOLDAY_NAME == holidayName)
                                     .Select(x => x.F_WEEK).Distinct().OrderBy(n => n).ToList();

            var flatDescs = (string.IsNullOrWhiteSpace(holidayName) || !weekNo.HasValue)
                            ? resorts.Select(x => x.F_FLAT_DESC).Distinct().OrderBy(n => n).ToList()
                            : resorts.Where(r => r.HOLDAY_NAME == holidayName && r.F_WEEK == weekNo)
                                     .Select(x => x.F_FLAT_DESC).Distinct().OrderBy(n => n).ToList();

            // Convert to SelectListItems
            ViewBag.HolidayNameList = holidayNames
                .Select(n => new SelectListItem { Text = n, Value = n }).ToList();

            ViewBag.WeekNoList = weekNos
                .Select(n => new SelectListItem { Text = n.ToString(), Value = n.ToString() }).ToList();

            ViewBag.FlatDescList = flatDescs
                .Select(n => new SelectListItem { Text = n, Value = n }).ToList();

            // Filtering
            if (!string.IsNullOrWhiteSpace(holidayName))
                resorts = resorts.Where(r => r.HOLDAY_NAME == holidayName);

            if (weekNo.HasValue)
                resorts = resorts.Where(r => r.F_WEEK == weekNo);

            if (!string.IsNullOrWhiteSpace(flatDesc))
                resorts = resorts.Where(r => r.F_FLAT_DESC == flatDesc);

            return View(resorts.ToList());
        }


        // GET: V_RESORT_NOT_RESERVED/Details/5
        public async Task<IActionResult> Details(short? holidayId, short? weekNo, short? flatId)
        {
            if (!holidayId.HasValue || !weekNo.HasValue || !flatId.HasValue)
                return NotFound();

            var resort =  _context.V_RESORT_NOT_RESERVED
                .Where(m =>
                    m.F_HCODE == holidayId &&
                    m.F_WEEK == weekNo &&
                    m.F_FCODE == flatId).ToList().FirstOrDefault();

            if (resort == null)
                return NotFound();

            var flatPics = await _context.V_FLAT_PICS
                .Where(m => m.F_CODE == flatId)
                .ToListAsync();

            resort.FlatPics = flatPics.Where(f => f.FLG == 1).ToList();
            resort.FlatVideos = flatPics.Where(f => f.FLG == 2).ToList();

            return View(resort);
        }

        public async Task<IActionResult> DetailsXXX(short? holidayId, short? weekNo, short? flatId)
        {
            if (holidayId == null || weekNo == null || flatId == null)
            {
                return NotFound();
            }

            var v_RESORT_NOT_RESERVED = await _context.V_RESORT_NOT_RESERVED
                .Where(m => m.F_HCODE == holidayId && m.F_WEEK == weekNo && m.F_FCODE == flatId).ToListAsync();
           
            if (v_RESORT_NOT_RESERVED == null)
            {
                return NotFound();
            }

            var flatPics = _context.V_FLAT_PICS
               .Where(m => m.F_CODE == flatId).ToList();

            // get flat pics
            v_RESORT_NOT_RESERVED[0].FlatPics = flatPics != null
             ? flatPics.Where(f => f.FLG == 1).ToList()
             : new List<V_FLAT_PIC>();

            // get flat videos
            v_RESORT_NOT_RESERVED[0].FlatVideos = flatPics != null
            ? flatPics.Where(f => f.FLG == 2).ToList()
            : new List<V_FLAT_PIC>();


            return View(v_RESORT_NOT_RESERVED[0]);
        }

        // GET: V_RESORT_NOT_RESERVED/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: V_RESORT_NOT_RESERVED/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_HCODE,F_WEEK,F_FCODE,HOLDAY_NAME,F_FLAT_DESC,F_WEEKCOST,F_INSURCOST,F_FROM,F_TO")] ResortsNotReserved v_RESORT_NOT_RESERVED)
        {
            if (ModelState.IsValid)
            {
                _context.Add(v_RESORT_NOT_RESERVED);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(v_RESORT_NOT_RESERVED);
        }

        // GET: V_RESORT_NOT_RESERVED/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_RESORT_NOT_RESERVED = await _context.V_RESORT_NOT_RESERVED.FindAsync(id);
            if (v_RESORT_NOT_RESERVED == null)
            {
                return NotFound();
            }
            return View(v_RESORT_NOT_RESERVED);
        }

        // POST: V_RESORT_NOT_RESERVED/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("F_HCODE,F_WEEK,F_FCODE,HOLDAY_NAME,F_FLAT_DESC,F_WEEKCOST,F_INSURCOST,F_FROM,F_TO")] ResortsNotReserved v_RESORT_NOT_RESERVED)
        {
            if (id != v_RESORT_NOT_RESERVED.F_HCODE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_RESORT_NOT_RESERVED);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_RESORT_NOT_RESERVEDExists(v_RESORT_NOT_RESERVED.F_HCODE))
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
            return View(v_RESORT_NOT_RESERVED);
        }

        // GET: V_RESORT_NOT_RESERVED/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_RESORT_NOT_RESERVED = await _context.V_RESORT_NOT_RESERVED
                .FirstOrDefaultAsync(m => m.F_HCODE == id);
            if (v_RESORT_NOT_RESERVED == null)
            {
                return NotFound();
            }

            return View(v_RESORT_NOT_RESERVED);
        }

        // POST: V_RESORT_NOT_RESERVED/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var v_RESORT_NOT_RESERVED = await _context.V_RESORT_NOT_RESERVED.FindAsync(id);
            if (v_RESORT_NOT_RESERVED != null)
            {
                _context.V_RESORT_NOT_RESERVED.Remove(v_RESORT_NOT_RESERVED);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_RESORT_NOT_RESERVEDExists(short id)
        {
            return _context.V_RESORT_NOT_RESERVED.Any(e => e.F_HCODE == id);
        }

    }
}
