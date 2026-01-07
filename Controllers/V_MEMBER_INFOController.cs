using ESSPMemberService.Data;
using ESSPMemberService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using reCAPTCHA.AspNetCore;
using System.Security.Cryptography;
using System.Text;



namespace ESSPMemberService.Controllers
{
    public class V_MEMBER_INFOController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRecaptchaService _recaptcha;
        private readonly IPermissionService _permissionService;
        

        public V_MEMBER_INFOController(ApplicationDbContext context, IRecaptchaService recaptcha, IPermissionService permissionService)
        {
            _context = context;
            _recaptcha = recaptcha;
            _permissionService = permissionService;
        }

        public ActionResult Login()
        {
            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}

            HttpContext.Session.SetString("MemID", "");
            HttpContext.Session.SetString("MemName", "");

            ViewBag.ErrorMsg = "";

            return View();
        }

        // GET: V_MEMBER_INFO
        public async Task<IActionResult> Index()
        {
            // TempData["MemID"] = "";
            ViewBag.ErrorMsg = "";

            return View();
           // return View(await _context.V_MEMBER_INFO.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(int MemID = 0, int ClassID = 0, string gRecaptchaResponse = "")
       {

            //if (!await IsCaptchaValid(gRecaptchaResponse))
            //{
            //    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed.");
            //    return View();
            //}

            //var recaptchaResult = await _recaptcha.Validate(Request);
            //if (!recaptchaResult.success)
            //{
            //    ModelState.AddModelError(string.Empty, "reCAPTCHA validation failed.");
            //    return View();
            //}
            try
            {
                
                HttpContext.Session.SetString("MemID", "");
                HttpContext.Session.SetString("MemName", "");
                ViewBag.ErrorMsg = "";
           

            if (MemID <= 0)
            {
                ViewBag.ErrorMsg = "يجب ادخال رقم القيد للعضو";
                return View();
            }

            //if (String.IsNullOrEmpty(ClassID))
            //{
            //    ViewBag.ErrorMsg = "يجب ادخال رقم الشعبة للعضو";
            //    return View();
            //}

            if (ClassID <= 0)
            {
                ViewBag.ErrorMsg += "يجب ادخال رقم الشعبة للعضو";
                return View();
            }

            var Result = _context.V_MEMBER_INFO.Where(m => m.F_CODE == MemID && m.F_BRANCHNO == ClassID).ToList().FirstOrDefault();

            //var Result = from m in data
            //             select m;

            //if (MemID>0 && !String.IsNullOrEmpty(ClassID))
            //{
            //    Result = Result.Where(s => s.F_CODE == MemID);
            //    Result = Result.Where(s => s.NOTES == ClassID);
            //}


            if (Result != null)
            {
                TempData["MemID"] = MemID.ToString();
                //TempData["MemName"] = Result.F_NAME.ToString();
                TempData["LatePay"] = Result.F_LATEPAY.ToString();
                TempData["PaYear"] = Result.F_PAYEAR.ToString();
                TempData["MainWrkSide"] = Result.F_MAINWRKSIDE.ToString();

                
                // Store the penalty value in the session
                HttpContext.Session.SetString("MemID", MemID.ToString());
                HttpContext.Session.SetString("MemName", Result.F_NAME.ToString());
               
                //HttpContext.Session.SetString("LatePay", Result.F_LATEPAY.ToString());
                //HttpContext.Session.SetString("PaYear", Result.F_PAYEAR.ToString());
                //HttpContext.Session.SetString("MainWrkSide", Result.F_MAINWRKSIDE.ToString());

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMsg = "تسجيل الدخول غير سليم";
                return View(); // return RedirectToAction("Index", "MemDes");
            }
            }
            catch(Exception ex) 
            {
                throw ex;
            }
     
        }

        public async Task<IActionResult> Logout()
        {
            // TempData["MemID"] = "";
            ViewBag.ErrorMsg = "";

            HttpContext.Session.SetString("MemID", "");
            HttpContext.Session.SetString("MemName", "");

            HttpContext.Session.SetString("UserID", "");
            HttpContext.Session.SetString("FullName", "");

            return RedirectToAction("Index", "Home");
        }

        // GET: V_MEMBER_INFO
        public async Task<IActionResult> LoginAdmin()
        {
            // TempData["MemID"] = "";
            ViewBag.ErrorMsg = "";

            return View();
            // return View(await _context.V_MEMBER_INFO.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> LoginAdmin(string userName, string password, string gRecaptchaResponse = "")
        {          
            try
            {
                HttpContext.Session.SetString("UserID", "");
                HttpContext.Session.SetString("FullName", "");
                ViewBag.ErrorMsg = "";

                if (userName == null)
                {
                    ViewBag.ErrorMsg = "يجب ادخال اسم المستخدم";
                    return View();
                }

                if (password == null)
                {
                    ViewBag.ErrorMsg = "يجب ادخال كلمة المرور";
                    return View();
                }
                //AppContext.SetSwitch("Switch.System.Globalization.UseNls", true);
                int hashCode = GetFixedHashCode(password);
                // password = password.GetHashCode().ToString();
                var Result = _context.V_USERS.Where(m => m.USER_NAME == userName && m.PASSWORD == password).ToList().FirstOrDefault();
                               
                if (Result != null)
                {
                    TempData["UserID"] = Result.USER_CODE.ToString();
                  
                    // Store the penalty value in the session
                    HttpContext.Session.SetString("UserID", Result.USER_CODE.ToString());
                    HttpContext.Session.SetString("FullName", Result.FULL_NAME.ToString());

                    //// Example after login
                    //var permissions = new List<string>
                    //                {
                    //                    "NEWS_VIEW",
                    //                    "NEWS_CREATE",
                    //                    "REQUEST_CREATE", "REQUEST_VIEW", "PAYMENT_REPORT"
                    //                };

                    //HttpContext.Session.SetString("UserPermissions", string.Join(",", permissions));

                    var permissions = _permissionService.GetUserPermissions(Result.USER_CODE);
                    HttpContext.Session.SetString("UserPermissions", string.Join(",", permissions));

                    return RedirectToAction("Admin", "Home");
                }
                else
                {
                    ViewBag.ErrorMsg = "تسجيل الدخول غير سليم";
                    return View(); // return RedirectToAction("Index", "MemDes");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        static int GetFixedHashCode(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToInt32(hashBytes, 0); // Convert first 4 bytes to int
            }
        }
        public ActionResult Main()
        {
            ViewBag.Message = "Your main page.";

            return View();
        }

        private async Task<bool> IsCaptchaValid(string gRecaptchaResponse)
        {
            var secretKey = "6Lc_aCMTAAAAABxrfU_Y4-C1LpuKb46W5ZgA2KFE"; // Use your actual secret key here
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={gRecaptchaResponse}");
            dynamic result = JsonConvert.DeserializeObject(response);
            return result.success == "true";
        }

        // GET: V_MEMBER_INFO/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_MEMBER_INFO = await _context.V_MEMBER_INFO
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (v_MEMBER_INFO == null)
            {
                return NotFound();
            }

            return View(v_MEMBER_INFO);
        }

        // GET: V_MEMBER_INFO/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: V_MEMBER_INFO/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_CODE,F_NAME,F_BRANCHNO,BRANCH,BRANCHGOV,F_LATEPAY,F_PAYEAR,F_MAINWRKSIDE")] V_MEMBER_INFO v_MEMBER_INFO)
        {
            if (ModelState.IsValid)
            {
                _context.Add(v_MEMBER_INFO);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(v_MEMBER_INFO);
        }

        // GET: V_MEMBER_INFO/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_MEMBER_INFO = await _context.V_MEMBER_INFO.FindAsync(id);
            if (v_MEMBER_INFO == null)
            {
                return NotFound();
            }
            return View(v_MEMBER_INFO);
        }

        // POST: V_MEMBER_INFO/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("F_CODE,F_NAME,F_BRANCHNO,BRANCH,BRANCHGOV,F_LATEPAY,F_PAYEAR,F_MAINWRKSIDE")] V_MEMBER_INFO v_MEMBER_INFO)
        {
            if (id != v_MEMBER_INFO.F_CODE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_MEMBER_INFO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_MEMBER_INFOExists(v_MEMBER_INFO.F_CODE))
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
            return View(v_MEMBER_INFO);
        }

        // GET: V_MEMBER_INFO/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_MEMBER_INFO = await _context.V_MEMBER_INFO
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (v_MEMBER_INFO == null)
            {
                return NotFound();
            }

            return View(v_MEMBER_INFO);
        }

        // POST: V_MEMBER_INFO/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var v_MEMBER_INFO = await _context.V_MEMBER_INFO.FindAsync(id);
            if (v_MEMBER_INFO != null)
            {
                _context.V_MEMBER_INFO.Remove(v_MEMBER_INFO);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_MEMBER_INFOExists(decimal id)
        {
            return _context.V_MEMBER_INFO.Any(e => e.F_CODE == id);
        }


        [HttpGet]
        public IActionResult GetMemberName(int id)
        {
            var member = _context.V_MEMBER_INFO
                .Where(m => m.F_CODE == id)
                .ToList().FirstOrDefault();

            if (member != null)
            {
                return Json(new { success = true, memberName = member.F_NAME });
            }
            return Json(new { success = false });
        }


    }
}
