using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ESSPMemberService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Oracle.ManagedDataAccess.Client;
using ESSPMemberService.Models.Tables;


namespace ESSPMemberService.Controllers
{
    public class V_T_DIFFMEMBERController : Controller
    {
        //[Route("[controller]")]
        //[ApiController]

        private readonly ApplicationDbContext _context;

        public V_T_DIFFMEMBERController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: V_T_DIFFMEMBER
        public async Task<IActionResult> Index()
        {

            //// Example of setting a session value
            //HttpContext.Session.SetString("MemID", "99999");
            try
            {
                long MemId = 0;
                try
                {
                    MemId = Convert.ToInt64(HttpContext.Session.GetString("MemID"));
                } 
                catch {
                    MemId = 0;
                }                               

                if (MemId <= 0)
                    return RedirectToAction("Index", "V_MEMBER_INFO");

                List<V_T_DIFFMEMBER> Result = new();

               // var lastPayDate = DateTime.Today.AddDays(-2);

                var LastPayment = _context.T_PAYMENT_MAIN
                                 .Where(m => m.F_MEMBER == MemId
                                     && m.F_IS_PROCESSED == 0).Select(e => e.F_ID)
                                 .ToList();

                //var LastPayment = await _context.T_PAYMENT_MAIN
                //    .FromSqlRaw("SELECT * FROM T_PAYMENT_MAIN WHERE F_MEMBER = :MemId AND F_ORDER_DATE >= :lastPayDate",
                //    new OracleParameter("MemId", MemId),
                //    new OracleParameter("lastPayDate", lastPayDate))
                //    .ToListAsync();

                if (LastPayment.Count > 0)
                {
                    ViewBag.PaymentMessage = "تم الدفع من قبل وتحت المراجعه";
                    return View(Result); // Replace "YourViewName" with the name of your view.

                    //   return RedirectToAction("Index", "V_MEMBER_INFO");
                }

                decimal total = 0;
                Result = _context.V_T_DIFFMEMBER.Where(m => m.F_CODE == MemId).ToList();

                // Add teh current year
                // F_VALUE + F_SERVICE_BOX + F_ESSP_CARD + تغمة الايصال (10 جنيه)
                // = 100+100+50+10 = 260
                //
                int currentYear = DateTime.Now.Year;

                var LatePay = Convert.ToInt32(TempData["LatePay"]); // HttpContext.Session.GetInt32("LatePay");
                var PaYear = Convert.ToInt32(TempData["PaYear"]);  // HttpContext.Session.GetInt32("PaYear");
                var MainWrkSide = Convert.ToInt32(TempData["MainWrkSide"]); // HttpContext.Session.GetInt32("MainWrkSide");
                
                foreach (var item in Result)
                {
                    total = total + item.VALUE.Value; // + penaltyValue;
                }

                decimal penaltyValue = 0;

                var penalty = _context.V_T_PENALTY.Where(m => m.F_FROMVAL <= total && m.F_TOVAL >= total).ToList().FirstOrDefault();
                if (penalty != null)
                    penaltyValue = penalty.F_PRCENT.Value /100;

                //ViewBag.penaltyValue = total * penaltyValue;
                //TempData["PenaltyValue"] = (total * penaltyValue).ToString();

                // Store the penalty value in the session
                HttpContext.Session.SetString("PenaltyValue", (total * penaltyValue).ToString());

                // Ensure TempData value is retained for the next request
                //TempData.Keep("PenaltyValue");

                if (MainWrkSide != 500 && MainWrkSide != 550)
                {
                    if (LatePay >= 0 && PaYear < currentYear)
                    {
                        var newDiffMember = new V_T_DIFFMEMBER
                        {
                            F_YEAR = Convert.ToInt16(currentYear),
                            VALUE = 260 // Add current year if not payed
                        };
                        Result.Add(newDiffMember);
                    }
                }


                ViewBag.Total_Value = total; // + (total * penaltyValue);
                return View(Result);
            }
            catch (Exception ex)
            {
                throw ex;
                return View();
                //return RedirectToAction("Index", "MemDes");
            }
        }

        // GET: V_T_DIFFMEMBER/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_DIFFMEMBER = await _context.V_T_DIFFMEMBER
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (v_DIFFMEMBER == null)
            {
                return NotFound();
            }

            return View(v_DIFFMEMBER);
        }

        // GET: V_T_DIFFMEMBER/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: V_T_DIFFMEMBER/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<int> selectedItemsxxx)
        {
            try
            {

                TempData.Keep("MemID");
                TempData.Keep("MemName");
                TempData.Keep("PenaltyValue");
                decimal totalValues = 0;

                // Retrieve the list of selected item values from the form
                var selectedItems = Request.Form["selectedItems"].ToList();


                // Prepare a list to store F_YEAR and VALUE tuples
                var selectedValues = new List<(int F_YEAR, decimal VALUE)>();

                // Check if any items were selected
                if (selectedItems != null && selectedItems.Any())
                {
                    foreach (var itemValue in selectedItems)
                    {
                        // Assuming VALUE is unique, we will fetch corresponding F_YEAR using VALUE
                        var valueKey = $"VALUE_{itemValue}";
                        var fYearKey = $"F_YEAR_{itemValue}";

                        // Attempt to retrieve the F_YEAR and VALUE from the form data
                        if (Request.Form.TryGetValue(fYearKey, out var fYearValue) &&
                            Request.Form.TryGetValue(valueKey, out var value))
                        {
                            // Convert string values to the correct data types
                            if (int.TryParse(fYearValue, out int fYear) && decimal.TryParse(value, out decimal decimalValue))
                            {
                                totalValues += decimalValue;
                                // Add the F_YEAR and VALUE to the list
                                selectedValues.Add((F_YEAR: fYear, VALUE: decimalValue));
                            }
                        }
                    }
                }
                    // You can now use the selectedValues list in your logic
                    // For example, passing it to a view or further processing
                    //return View(selectedValues);

                    TempData["TotalValues"] = totalValues;
                    return View();                
            }
            catch (Exception ex)
            {
                throw ex;
                return View();
            }
        }

        // GET: V_T_DIFFMEMBER/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_DIFFMEMBER = await _context.V_T_DIFFMEMBER.FindAsync(id);
            if (v_DIFFMEMBER == null)
            {
                return NotFound();
            }
            return View(v_DIFFMEMBER);
        }

        // POST: V_T_DIFFMEMBER/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("F_CODE,F_YEAR,DATE_DIFF,VALUE,NOTES,F_ISPAY_PRCENT")] V_T_DIFFMEMBER v_DIFFMEMBER)
        {
            if (id != v_DIFFMEMBER.F_CODE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(v_DIFFMEMBER);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!V_DIFFMEMBERExists(v_DIFFMEMBER.F_CODE))
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
            return View(v_DIFFMEMBER);
        }

        // GET: V_T_DIFFMEMBER/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_DIFFMEMBER = await _context.V_T_DIFFMEMBER
                .FirstOrDefaultAsync(m => m.F_CODE == id);
            if (v_DIFFMEMBER == null)
            {
                return NotFound();
            }

            return View(v_DIFFMEMBER);
        }

        // POST: V_T_DIFFMEMBER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var v_DIFFMEMBER = await _context.V_T_DIFFMEMBER.FindAsync(id);
            if (v_DIFFMEMBER != null)
            {
                _context.V_T_DIFFMEMBER.Remove(v_DIFFMEMBER);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool V_DIFFMEMBERExists(int id)
        {
            return _context.V_T_DIFFMEMBER.Any(e => e.F_CODE == id);
        }
    }
}
