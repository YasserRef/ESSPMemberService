using ESSPMemberService.Data;
using ESSPMemberService.Helper;
using ESSPMemberService.Models.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

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
            return View(await _context.T_NEWS.Where(e => e.F_ACTIVE == 1)
                 .OrderByDescending(e => e.F_CREATED_DATE).ToListAsync());
        }

        public async Task<IActionResult> Show(string title, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.T_NEWS.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                // تقسيم النص إلى كلمات
                var words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                               //  .Select(w => Arabic.NormalizeArabic(w).ToLower())
                                 .ToArray();

                foreach (var word in words)
                {
                    query = query.Where(x => EF.Functions.Like(x.F_TITLE, $"%{word}%"));
                }

                //query = query.Where(x =>
                //    x.F_TITLE != null &&
                //    words.All(word => Arabic.NormalizeArabic(x.F_TITLE).ToLower().Contains(word))
                //);

                //query = query.Where(x => x.F_TITLE != null && words.Contains(x.F_TITLE) );
            }

            if (fromDate.HasValue)
                query = query.Where(x => x.F_CREATED_DATE >= fromDate.Value);

            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.F_CREATED_DATE <= endOfDay);
            }
               

            // .Where(e => e.F_ACTIVE == 1)
            var data = await query.OrderByDescending(x => x.F_CREATED_DATE).ToListAsync();

            return View(data);
        }

        // GET: T_NEWS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_New = _context.T_NEWS
                .Where(m => m.F_ID == id).ToList().FirstOrDefault();

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
        public async Task<IActionResult> Create(T_NEWS model, IFormFile newImage)
        {
            model.F_CREATED_DATE = DateTime.Now;
            if (newImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(newImage.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("", "نوع الصورة غير مدعوم");
                    return View(model);
                }

                if (newImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "حجم الصورة يجب ألا يزيد عن 2 ميجا");
                    return View(model);
                }
            }

            // Save Image
            var folderPath = Path.Combine("wwwroot","assets","img","news");

            Directory.CreateDirectory(folderPath);           

            if (ModelState.IsValid)
            {
                model.F_ID = await _context.T_NEWS.MaxAsync(e => e.F_ID) + 1;

                var fileName = $"{model.F_ID}_{DateTime.Now:yyyy}_{DateTime.Now:MM}_{DateTime.Now:dd}{Path.GetExtension(newImage.FileName)}";
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }

                model.F_IMAGE_URL = $"/assets/img/news/{fileName}";

                var sql = "INSERT INTO T_NEWS (F_ID,F_TITLE,F_CONTENT,F_IMAGE_URL,F_CREATED_DATE,F_ACTIVE,F_NAME) VALUES (:p0,:p1,:p2,:p3,:p4,:p5,:p6)";

                var entity = await _context.Database.ExecuteSqlRawAsync(sql,
                    new OracleParameter("p0", model.F_ID),
                    new OracleParameter("p1", model.F_TITLE),
                    new OracleParameter("p2", model.F_CONTENT),
                    new OracleParameter("p3", model.F_IMAGE_URL),
                    new OracleParameter("p4", model.F_CREATED_DATE),
                    new OracleParameter("p5", model.F_ACTIVE),
                    new OracleParameter("p6", model.F_NAME)
                    );

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Show));
            }
            return View(model);
        }

        // GET: T_NEWS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t_New = _context.T_NEWS.Where(e => e.F_ID == id).ToList().FirstOrDefault();
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
        public async Task<IActionResult> EditOld(int id, [Bind("F_ID,F_TITLE,F_CONTENT,F_IMAGE_URL,F_CREATED_DATE,F_ACTIVE")] T_NEWS model)
        {
            if (id != model.F_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //, F_IMAGE_URL = :p4, F_CREATED_DATE = :p5
                    var sql = @" UPDATE T_NEWS SET F_TITLE = :p1, F_CONTENT = :p2, F_ACTIVE = :p3 WHERE F_ID = :p0";

                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new OracleParameter("p0", model.F_ID),
                        new OracleParameter("p1", model.F_TITLE),
                        new OracleParameter("p2", model.F_CONTENT),
                       // new OracleParameter("p4", model.F_IMAGE_URL),
                       // new OracleParameter("p5", model.F_CREATED_DATE),
                        new OracleParameter("p3", model.F_ACTIVE));

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T_NewExists(model.F_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Show));
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, T_NEWS model, IFormFile newImage)
        {
            if (id != model.F_ID)
                return NotFound();

            ModelState.Remove("newImage");

            if (!ModelState.IsValid)
                return View(model);

            var news = _context.T_NEWS.Where(e => e.F_ID == id).ToList().FirstOrDefault();
            if (news == null)
                return NotFound();

            //// تحديث الحقول
            //news.F_NAME = model.F_NAME;
            //news.F_TITLE = model.F_TITLE;
            //news.F_CONTENT = model.F_CONTENT;
            //news.F_ACTIVE = model.F_ACTIVE;

            // في حال تم رفع صورة جديدة
            if (newImage != null && newImage.Length > 0)
            {
                var ext = Path.GetExtension(newImage.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "نوع الصورة غير مدعوم");
                    return View(model);
                }

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","assets","img","news");

                Directory.CreateDirectory(folderPath);
               
                var fileName = $"{model.F_ID}_{DateTime.Now:yyyy}_{DateTime.Now:MM}_{DateTime.Now:dd}{ext}";
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }

                model.F_IMAGE_URL = $"/assets/img/news/{fileName}";
            }

            var sql = @" UPDATE T_NEWS SET F_TITLE = :p1, F_CONTENT = :p2, F_ACTIVE = :p3, F_IMAGE_URL = :p4, F_NAME = :p5 WHERE F_ID = :p0";

            await _context.Database.ExecuteSqlRawAsync(sql,
                new OracleParameter("p0", model.F_ID),
                new OracleParameter("p1", model.F_TITLE),
                new OracleParameter("p2", model.F_CONTENT),              
                new OracleParameter("p3", model.F_ACTIVE),
                new OracleParameter("p4", model.F_IMAGE_URL),
                new OracleParameter("p5", model.F_NAME)
                // new OracleParameter("p5", model.F_CREATED_DATE),
                );

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Show));
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
