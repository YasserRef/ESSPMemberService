using BotDetect;
using ESSPMemberService.Attributes;
using ESSPMemberService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ESSPMemberService.Controllers
{
    public class V_REQUESTSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public V_REQUESTSController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: V_REQUESTS
        public async Task<IActionResult> Index()
        {
            long MemId = 0;
            try
            {
                MemId = Convert.ToInt64(HttpContext.Session.GetString("MemID"));
                if (MemId > 0)  
                    return View(await _context.V_REQUESTS.Where(e => e.F_MEM_ID == MemId && e.F_STATUS == 0).ToListAsync());

            }
            catch
            {
                MemId = 0;
            }           

            //if (MemId <= 0)
                return RedirectToAction("Index", "V_MEMBER_INFO");

            //return View(await _context.V_REQUESTS.Where(e => e.F_MEM_ID == MemId && e.F_STATUS == 0).ToListAsync());
        }


        public async Task<IActionResult> IndexAdmin(int? memId, DateTime? fromDate, DateTime? toDate, string status, bool? isUser)
        {

            int userId = 0;

            try
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            }
            catch
            {
                return RedirectToAction("LoginAdmin", "V_MEMBER_INFO");
            }


            var requests = _context.V_REQUESTS.AsQueryable();

            //if (userId > 0)
            //    requests = requests.Where(e => e.F_USER_ID == userId && e.F_STATUS == 0);

            if (memId.HasValue)
                requests = requests.Where(r => r.F_MEM_ID == memId.Value);

            if (fromDate.HasValue)
                requests = requests.Where(r => r.F_REQUEST_DATE >= fromDate.Value);

            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
                requests = requests.Where(r => r.F_REQUEST_DATE <= endOfDay);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Open")
                    requests = requests.Where(r => r.F_STATUS == 0);
                else if (status == "Close")
                    requests = requests.Where(r => r.F_STATUS == 1);
            }

            if (isUser != null)
            {
                if (isUser == false)
                    requests = requests.Where(r => r.F_USER_ID == 0);
                else if (isUser == true)
                    requests = requests.Where(r => r.F_USER_ID > 0);
            }

            return View(await requests.ToListAsync());
        }   


        // GET: V_REQUESTS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_REQUESTS = await _context.V_REQUESTS.Where(e => e.F_ID == id).ToListAsync();
            if (v_REQUESTS == null)
            {
                return NotFound();
            }

            return View(v_REQUESTS[0]);
        }
        public async Task<IActionResult> DetailsAdmin(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_REQUESTS = await _context.V_REQUESTS.Where(e => e.F_ID == id).ToListAsync();
            if (v_REQUESTS == null)
            {
                return NotFound();
            }

            return View(v_REQUESTS[0]);
        }

        // GET: V_REQUESTS/Create
        public IActionResult CreateRequest()
        {
            int userId = 0;
            try
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            }
            catch
            {
                userId = 0;
            }

            if (userId <= 0)
                return RedirectToAction("LoginAdmin", "V_MEMBER_INFO");

            //var request =  _context.T_REQUESTS.Where(e => e.F_MEM_ID == MemId && e.F_STATUS == 0).ToList();
            //if (request.Count > 0)
            //{
            //    return RedirectToAction("Details", "V_REQUESTS", new { id = request[0].F_ID });
            //}

            return View();

        }

        // POST: V_REQUESTS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest([Bind("F_MEM_ID,F_MEM_NAME,F_REQUEST_ID,F_REQUEST_DATE,F_MOBILE_NO,F_NATIONAL_ID,F_TITTLE,F_NOTES")] T_REQUESTS model)
        {

            if (ModelState.IsValid)
            {              
                model.F_REQUEST_DATE = DateTime.Now;
                model.F_REQUEST_ID = _context.V_REQUESTS.Count() + 1;
                model.F_USER_ID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

                var sql = @"INSERT INTO T_REQUESTS (F_MEM_ID,F_MEM_NAME,F_REQUEST_ID,F_REQUEST_DATE,F_MOBILE_NO,F_NATIONAL_ID,F_TITTLE,F_NOTES,F_USER_ID)
                                                    VALUES (:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9) 
                                                    RETURNING F_ID INTO :p10";

                var idParam = new OracleParameter("p10", OracleDbType.Int32, ParameterDirection.Output);

                await _context.Database.ExecuteSqlRawAsync(sql,
                    new OracleParameter("p1", model.F_MEM_ID),
                    new OracleParameter("p2", model.F_MEM_NAME),
                    new OracleParameter("p3", model.F_REQUEST_ID),
                    new OracleParameter("p4", model.F_REQUEST_DATE),
                    new OracleParameter("p5", model.F_MOBILE_NO),
                    new OracleParameter("p6", model.F_NATIONAL_ID),
                    new OracleParameter("p7", model.F_TITTLE),
                    new OracleParameter("p8", model.F_NOTES),
                    new OracleParameter("p9", model.F_USER_ID),
                    idParam
                );

                //var insertedId = (int)idParam.Value;
                return RedirectToAction(nameof(DetailsAdmin), new { id = idParam.Value });
            }

            return View(model);
        }


        // GET: V_REQUESTS/Create
        // [HasPermission("REQUEST_CREATE")]
        public IActionResult Create()
        {
            long MemId = 0;
            try
            {
                MemId = Convert.ToInt64(HttpContext.Session.GetString("MemID"));
                var request = _context.T_REQUESTS.Where(e => e.F_MEM_ID == MemId && e.F_STATUS == 0).ToList();
                if (request.Count > 0)
                {
                    return RedirectToAction("Details", "V_REQUESTS", new { id = request[0].F_ID });
                }
            }
            catch
            {
                MemId = 0;
            }

            if (MemId <= 0)
                return RedirectToAction("Index", "V_MEMBER_INFO");            

            return View();
        }

        // POST: V_REQUESTS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_REQUEST_ID,F_REQUEST_DATE,F_MOBILE_NO,F_NATIONAL_ID,F_TITTLE,F_NOTES")] T_REQUESTS model)
        {
          
            if (ModelState.IsValid)
            {
                model.F_MEM_ID = Convert.ToInt64(HttpContext.Session.GetString("MemID"));
                model.F_MEM_NAME = HttpContext.Session.GetString("MemName");
                model.F_REQUEST_DATE = DateTime.Now;
                model.F_REQUEST_ID = _context.V_REQUESTS.Count() + 1;

                var sql = @"INSERT INTO T_REQUESTS (F_MEM_ID,F_MEM_NAME,F_REQUEST_ID,F_REQUEST_DATE,F_MOBILE_NO,F_NATIONAL_ID,F_TITTLE,F_NOTES)
                                                    VALUES (:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8) 
                                                    RETURNING F_ID INTO :p9";

                var idParam = new OracleParameter("p9", OracleDbType.Int32, ParameterDirection.Output);

                await _context.Database.ExecuteSqlRawAsync(sql,
                    new OracleParameter("p1", model.F_MEM_ID),
                    new OracleParameter("p2", model.F_MEM_NAME),
                    new OracleParameter("p3", model.F_REQUEST_ID),
                    new OracleParameter("p4", model.F_REQUEST_DATE),
                    new OracleParameter("p5", model.F_MOBILE_NO),
                    new OracleParameter("p6", model.F_NATIONAL_ID),
                    new OracleParameter("p7", model.F_TITTLE),
                    new OracleParameter("p8", model.F_NOTES),
                    idParam
                );

                //var insertedId = (int)idParam.Value;
                return RedirectToAction(nameof(Details), new { id = idParam.Value });

            }


            return View(model);          
        }

        // GET: V_REQUESTS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_REQUESTS = await _context.V_REQUESTS.Where(e=>e.F_ID == id).ToListAsync();
            if (v_REQUESTS == null)
            {
                return NotFound();
            }
            return View(v_REQUESTS[0]);
        }

        // POST: V_REQUESTS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("F_ID,F_MEM_ID,F_GUIDE_ID,F_MEM_NAME,F_REQUEST_ID,F_REQUEST_DATE,F_MOBILE_NO,F_NATIONAL_ID,F_TITTLE,F_NOTES")] V_REQUESTS v_REQUESTS)
        {
            if (id != v_REQUESTS.F_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_REQUESTS);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_REQUESTSExists(v_REQUESTS.F_ID))
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
            return View(v_REQUESTS);
        }

        // GET: V_REQUESTS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_REQUESTS = await _context.V_REQUESTS.Where(e => e.F_ID == id).ToListAsync();
            if (v_REQUESTS == null)
            {
                return NotFound();
            }

            return View(v_REQUESTS);
        }

        // POST: V_REQUESTS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var v_REQUESTS = await _context.V_REQUESTS.Where(e => e.F_ID == id).ToListAsync();
            if (v_REQUESTS != null)
            {
                _context.V_REQUESTS.Remove(v_REQUESTS[0]);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_REQUESTSExists(int id)
        {
            return _context.V_REQUESTS.Any(e => e.F_ID == id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePrintDate([FromBody] RequestIdDto dto)
        {

            var sql = @"Update T_REQUESTS Set F_PRINT_DATE=:p1 Where F_ID=:p2";
                      
             _context.Database.ExecuteSqlRawAsync(sql,
                new OracleParameter("p1", DateTime.Now),
                new OracleParameter("p2", dto.Id)
            );

            //var request = _context.T_REQUESTS.Where(r => r.F_ID == dto.Id).ToList();
            //if (request != null)
            //{
            //    request[0].F_PRINT_DATE = DateTime.Now;
            //    _context.SaveChanges();
            //    return Json(new { success = true });
            //}
            return Json(new { success = true });
            //return Json(new { success = false });
        }


        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Update_Status([FromBody] RequestIdDto dto)
        {

            var sql = @"Update T_REQUESTS Set F_STATUS=:p1 Where F_ID=:p2";

            _context.Database.ExecuteSqlRawAsync(sql,
               new OracleParameter("p1", 1),
               new OracleParameter("p2", dto.Id)
           );
         
            return Json(new { success = true });
        }




    }
}
