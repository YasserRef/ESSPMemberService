using ESSPMemberService.Data;
using ESSPMemberService.Models.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ESSPMemberService.Controllers
{
    public class PAYMENT_BANKController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public PAYMENT_BANKController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        // GET: T_PAYMENT_MAIN
        public async Task<IActionResult> Index()
        {
            return View(await _context.T_PAYMENT_MAIN.ToListAsync());
        }

        // GET: T_PAYMENT_MAIN/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var result = await _context.T_PAYMENT_MAIN
                                          .Where(e => e.F_ID == id)
                                          .Select(n => new T_PAYMENT_MAIN
                                          {
                                              F_ID = n.F_ID,
                                              F_TOTAL_VALUE = n.F_TOTAL_VALUE,
                                              F_PAYMENT_METHOD_ID = n.F_PAYMENT_METHOD_ID,                                             
                                              F_CARD = n.F_CARD,
                                              F_ORDER_NO = n.F_ORDER_NO,
                                              F_ORDER_YEAR = n.F_ORDER_YEAR,
                                              F_ORDER_DATE = n.F_ORDER_DATE,                                             
                                          })
                                          .ToListAsync();

                await DisplayImage(id);

                if (result[0] == null)
                {
                    return NotFound();
                }
                return View(result[0]);
            }
            catch (OracleException ex)
            {
                // Log or handle the OracleException
                Console.WriteLine($"OracleException: {ex.Message}");
                throw ex;
            }

            catch (Exception ex)
            {
                // Log or handle the OracleException
                Console.WriteLine($"OracleException: {ex.Message}");
                throw ex;
            }

        }

        public async Task<IActionResult> DisplayImageXXX(int id)
        {
        if (id <= 0)
            return NotFound();

        try
        {
                var entity = await _context.T_PAYMENT_MAIN
                        .SingleOrDefaultAsync(e => e.F_ID == id);

                if (entity?.F_PAY_IMAGE == null || entity.F_PAY_IMAGE.Length == 0)
                    return NotFound();

                string base64Image = Convert.ToBase64String(entity.F_PAY_IMAGE);
                string imgSrc = $"data:image/png;base64,{base64Image}";
                ViewBag.ImageSrc = imgSrc;

                return View();

            }
            catch (Exception ex)
        {
            // Log and handle exceptions
            return StatusCode(500, "An error occurred: " + ex.Message);
        }
    }


        /// <summary>
        /// Asynchronously retrieves and streams a BLOB image from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the payment image to retrieve.</param>
        /// <returns>An IActionResult containing the image file stream.</returns>
        public async Task<IActionResult> DisplayImage1(int id)
        {
            if (id <= 0)
                return NotFound();

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr))
                return StatusCode(500, "Database connection string is missing.");

            try
            {
                await using var conn = new OracleConnection(connStr);
                await conn.OpenAsync();

                const string sql = @"SELECT F_PAY_IMAGE FROM ESSP_MOBILE.T_PAYMENT_MAIN WHERE F_ID = :id";

                await using var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add(new OracleParameter("id", id));

                await using var reader = cmd.ExecuteReader();
                if (reader == null || reader.IsClosed)
                {
                    return StatusCode(500, "Error: Failed to create a data reader.");
                }
                if (!reader.Read())
                {
                    return NotFound(); // No rows returned
                }

                if (reader.IsDBNull(0))
                {
                    return NotFound(); // BLOB column is NULL
                }

                if (await reader.ReadAsync())
                {
                    await using var blob = reader.GetOracleBlob(0);

                    if (!blob.IsNull)
                    {
                        // Option A: Return blob directly
                        return File(blob, "image/jpeg");

                        // Option B: Copy into MemoryStream if needed
                        // var ms = new MemoryStream();
                        // await blob.CopyToAsync(ms);
                        // ms.Position = 0;
                        // return File(ms, "image/jpeg");
                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                // TODO: replace with log4net or your MongoDB log
                Console.WriteLine($"Error retrieving image: {ex}");
                return StatusCode(500, "An error occurred while retrieving the image.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DisplayImage(int id)
        {
            if (id <= 0)
                return NotFound();

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            await using var conn = new OracleConnection(connStr);
            await conn.OpenAsync();

            // First query: get LOB length
            var lengthCmd = new OracleCommand(
                "SELECT DBMS_LOB.GETLENGTH(F_PAY_IMAGE) FROM ESSP_MOBILE.T_PAYMENT_MAIN WHERE F_ID = :id",
                conn);
            lengthCmd.Parameters.Add(new OracleParameter("id", id));

            var lengthObj = await lengthCmd.ExecuteScalarAsync();
            if (lengthObj == DBNull.Value || lengthObj == null)
                return NotFound();

            long length = Convert.ToInt64(lengthObj);
            if (length == 0)
                return NotFound();

            // Fetch in 32k chunks
            var buffer = new List<byte>();
            long pos = 1; // LOB positions are 1-based in Oracle
            const int chunkSize = 2000;

            while (pos <= length)
            {
                var sql = "SELECT DBMS_LOB.SUBSTR(F_PAY_IMAGE, :len, :pos) FROM ESSP_MOBILE.T_PAYMENT_MAIN WHERE F_ID = :id";
                var cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add(new OracleParameter("len", Math.Min(chunkSize, length - pos + 1)));
                cmd.Parameters.Add(new OracleParameter("pos", pos));
                cmd.Parameters.Add(new OracleParameter("id", id));

                var chunk = (byte[])await cmd.ExecuteScalarAsync();
                buffer.AddRange(chunk);

                pos += chunkSize;
            }

            var bytes = buffer.ToArray();

            // Return as PNG (or detect type if needed)
            return File(bytes, "image/png");
        }



        // GET: T_PAYMENT_MAIN/Create
        public IActionResult Create()
        {
            var model = new T_PAYMENT
            {
                F_COMPANY_NO = 1,
                F_ORDER_YEAR = DateTime.Now.Year, // Set the default year to the current year
                F_ORDER_DATE = DateTime.Now
            };

            // Assuming GetCompaniesAsync() is an asynchronous method that returns a List<T_PAYMENT_COMPANY>
            var companies = _context.T_PAYMENT_COMPANY.ToList();

            ViewBag.CompanyList = companies.Select(c => new SelectListItem
            {
                Value = c.F_CODE.ToString(),
                Text = c.F_NAME
            }).ToList();

            return View(model);
        }

        // POST: T_PAYMENT_MAIN/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("F_ORDER_NO,F_COMPANY_NO,F_ORDER_YEAR,F_MEMBER,F_TOTAL_VALUE,F_REFERENCE_NO,F_PAYMENT,F_HAFZA_NO,F_HAFZA_YEAR,F_BANK_NAME,F_CARD,F_ORDER_DATE,F_PAY_FEES,F_POST_COST,F_PENALTY")] T_PAYMENT model)
        {
            if (ModelState.IsValid)
            {
                model.F_ID = await _context.T_PAYMENT_MAIN.MaxAsync(e => e.F_ID) + 1;
                // _context.Add(T_PAYMENT_MAIN);
                model.F_MEMBER = 111;
                model.F_PENALTY = 0;
                model.F_ORDER_NO = 1;

                var sql = "INSERT INTO T_PAYMENT_MAIN (F_ID,F_ORDER_NO,F_COMPANY_NO,F_ORDER_YEAR,F_ORDER_DATE,F_MEMBER" +
                     ", F_TOTAL_VALUE,F_REFERENCE_NO,F_PAYMENT,F_CARD,F_PAY_FEES,F_POST_COST,F_PENALTY" +
                    // ",F_HAFZA_NO,F_HAFZA_YEAR," +
                    ")" +
                    " VALUES (:p0,:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p10,:p11,:p12,:p13" +                   
                    // ",:p14,:p15" +
                    ")";

                var entity = await _context.Database.ExecuteSqlRawAsync(sql,
                    new OracleParameter("p0", model.F_ID),
                    new OracleParameter("p1", model.F_ORDER_NO),
                    new OracleParameter("p2", model.F_COMPANY_NO),
                    new OracleParameter("p3", model.F_ORDER_YEAR),
                    new OracleParameter("p4", model.F_ORDER_DATE),
                    new OracleParameter("p5", model.F_MEMBER),
                    new OracleParameter("p6", model.F_TOTAL_VALUE),
                    new OracleParameter("p7", model.F_REFERENCE_NO),
                    new OracleParameter("p8", model.F_PAYMENT),                                             
                    //new OracleParameter("p9", model.F_BANK_NAME),
                    new OracleParameter("p10", model.F_CARD),
                    new OracleParameter("p11", model.F_PAY_FEES),
                    new OracleParameter("p12", model.F_POST_COST),
                    new OracleParameter("p13", model.F_PENALTY)
                    // new OracleParameter("p14", model.F_HAFZA_NO),
                    // new OracleParameter("p15", model.F_HAFZA_YEAR) 
                    );

              await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = model.F_ID });
            }
           

            return View(model);
        }

        // GET: T_PAYMENT_MAIN/Create
        [HttpGet("CreateCash")]
        public IActionResult CreateCash()
        {
            var model = new T_PAYMENT_MAIN
            {
               // F_COMPANY_NO = 1,
                F_ORDER_YEAR = DateTime.Now.Year, // Set the default year to the current year
                F_ORDER_DATE = DateTime.Now
            };

            // Assuming GetCompaniesAsync() is an asynchronous method that returns a List<T_PAYMENT_COMPANY>
            var companies = _context.T_PAYMENT_COMPANY.ToList();

            ViewBag.CompanyList = companies.Select(c => new SelectListItem
            {
                Value = c.F_CODE.ToString(),
                Text = c.F_NAME
            }).ToList();

            return View(model);
        }

        [HttpPost("CreateCash")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCash([Bind("F_ORDER_NO,F_COMPANY_NO,F_ORDER_YEAR,F_MEMBER,F_TOTAL_VALUE,F_REFERENCE_NO,F_PAYMENT,F_HAFZA_NO,F_HAFZA_YEAR,F_BANK_NAME,F_CARD,F_ORDER_DATE,F_PAY_FEES,F_POST_COST,F_PENALTY")] T_PAYMENT_CASH model)
        {
            if (ModelState.IsValid)
            {
                model.F_ID = await _context.T_PAYMENT_MAIN.MaxAsync(e => e.F_ID) + 1;
                // _context.Add(T_PAYMENT_MAIN);
                model.F_MEMBER = 111;
                model.F_PENALTY = 0;
                model.F_ORDER_NO = 1;

                var sql = "INSERT INTO T_PAYMENT_MAIN (F_ID,F_ORDER_NO,F_COMPANY_NO,F_ORDER_YEAR,F_ORDER_DATE,F_MEMBER" +
                     ", F_TOTAL_VALUE,F_REFERENCE_NO,F_PAYMENT,F_PAY_FEES,F_POST_COST,F_PENALTY" +
                    // ",F_HAFZA_NO,F_HAFZA_YEAR," +
                    ")" +
                    " VALUES (:p0,:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p11,:p12,:p13" +
                    // ",:p14,:p15" +
                    ")";

                var entity = await _context.Database.ExecuteSqlRawAsync(sql,
                    new OracleParameter("p0", model.F_ID),
                    new OracleParameter("p1", model.F_ORDER_NO),
                    new OracleParameter("p2", model.F_COMPANY_NO),
                    new OracleParameter("p3", model.F_ORDER_YEAR),
                    new OracleParameter("p4", model.F_ORDER_DATE),
                    new OracleParameter("p5", model.F_MEMBER),
                    new OracleParameter("p6", model.F_TOTAL_VALUE),
                    new OracleParameter("p7", model.F_REFERENCE_NO),
                    new OracleParameter("p8", model.F_PAYMENT),
                    //new OracleParameter("p9", model.F_BANK_NAME),
                    //new OracleParameter("p10", model.F_CARD),
                    new OracleParameter("p11", model.F_PAY_FEES),
                    new OracleParameter("p12", model.F_POST_COST),
                    new OracleParameter("p13", model.F_PENALTY)
                    // new OracleParameter("p14", model.F_HAFZA_NO),
                    // new OracleParameter("p15", model.F_HAFZA_YEAR) 
                    );

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = model.F_ID });
            }


            return View(model);
        }


        #region Bank Payment
        [HttpPost("CreateBank")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBank(List<int> selectedItems)
        {
             TempData.Keep("MemID");          

            var obj =  GetValues(selectedItems);

            if (obj.TotalValues <= 0)
                return NoContent();

            var penaltyValueString = HttpContext.Session.GetString("PenaltyValue");
            decimal penaltyValue = string.IsNullOrEmpty(penaltyValueString) ? 0 : Convert.ToDecimal(penaltyValueString);

            //obj.TotalValues += penaltyValue;

            // Store the updated list back in TempData            
            //TempData["TotalValues"] = obj.TotalValues;
            TempData["SelectedValues"] = JsonConvert.SerializeObject(obj.SelectedValues);         

            if (obj.SelectedValues.Last().F_YEAR == DateTime.Today.Year )
            {
                return RedirectToAction(nameof(SendMemCard), new { totalValue = obj.TotalValues + penaltyValue, F_VALUE = obj.TotalValues, F_PENALTY_VALUE = penaltyValue });
            }

            ViewBag.PaymentMethods = new SelectList(
                                  _context.V_SYSCODE
                                 .Where(t => t.F_TYPE == 27)
                                      .Select(e => new
                                      {
                                          Value = e.F_CODE,
                                          Text = e.F_NAME
                                      })
                                      .ToList(),
                                  "Value",
                                  "Text"
                              );

            var model = new T_PAYMENT_MAIN
            {
                F_VALUE = obj.TotalValues,
                F_PENALTY_VALUE = penaltyValue,
                F_TOTAL_VALUE = obj.TotalValues + penaltyValue,
                F_ORDER_YEAR = DateTime.Now.Year, // Set the default year to the current year
                F_ORDER_DATE = DateTime.Now,
                // F_BANK_NAME = "البنك الاهلي المصري – الفرع الرئيسي",
                F_CARD = "" //"0013070556722601012"
            };

            // ============================================================
            //    // Create the view model
            //    var viewModel = new YourViewModel
            //    {
            //        Model = model,
            //        SelectedValues = obj.SelectedValues
            //};

            //// Assuming GetCompaniesAsync() is an asynchronous method that returns a List<T_PAYMENT_COMPANY>
            //var companies = _context.T_PAYMENT_COMPANY.ToList();

            //ViewBag.CompanyList = companies.Select(c => new SelectListItem
            //{
            //    Value = c.F_CODE.ToString(),
            //    Text = c.F_NAME
            //}).ToList();

            return View(model);
        }


        [HttpPost("SaveBank")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBank( T_PAYMENT_MAIN model, IFormFile payImage)
        {

            // model.F_TOTAL_VALUE = Convert.ToDecimal(TempData["TotalValues"]) + Convert.ToDecimal(TempData["CardValue"]);
            // model.F_TOTAL_VALUE = model.F_TOTAL_VALUE + model.PaymentMemCard.F_SEND_COST;

            model.F_ORDER_YEAR = DateTime.Now.Year; // Set the default year to the current year
            model.F_ORDER_DATE = DateTime.Now;
            //model.F_BANK_NAME = "البنك الاهلي المصري – الفرع الرئيسي";
            //model.F_CARD = "0013070556722601012"; 
            try
            {

                // F_IS_PROCESSED
                var exist = await _context.T_PAYMENT_MAIN
                    .Where(e => e.F_IS_PROCESSED == 0
                    && e.F_MEMBER == model.F_MEMBER)
                    .Select(e => e.F_ID).ToListAsync();

                if (exist != null)
                {
                    ModelState.AddModelError(string.Empty, "تم ادخال بيانات العضو من قبل وجارى المراجعه والتنفيذ");
                    return View("SendMemCard", model);
                }

                if (ModelState.IsValid)
                {
                    // =====================================================================
                    try
                    {
                        model.F_ID = await _context.T_PAYMENT_MAIN.MaxAsync(e => e.F_ID) + 1;
                    }
                    catch
                    {
                        model.F_ID = 1;
                    }

                    try
                    {
                        model.F_ORDER_NO = await _context.T_PAYMENT_MAIN.Where(e => e.F_ORDER_YEAR == model.F_ORDER_YEAR).MaxAsync(e => e.F_ORDER_NO) + 1;
                    }
                    catch
                    {
                        model.F_ORDER_NO = 1;
                    }

                // _context.Add(T_PAYMENT_MAIN);
                model.F_MEMBER = Convert.ToInt32(HttpContext.Session.GetString("MemID")); 
                //model.F_PENALTY = 0;
                model.F_ORDER_YEAR = DateTime.Now.Year; // Set the default year to the current year
                model.F_ORDER_DATE = DateTime.Now;
                model.F_SEND_COST = model.PaymentMemCard != null ? model.PaymentMemCard.F_SEND_COST.Value : 0;
                //model.F_BANK_NAME = "البنك الاهلي المصري – الفرع الرئيسي";
                //model.F_CARD = "0013070556722601012";
                // ==============================================================
                if (payImage != null && payImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await payImage.CopyToAsync(memoryStream);
                        model.F_PAY_IMAGE = memoryStream.ToArray();
                    }
                }

                var sql = "INSERT INTO T_PAYMENT_MAIN (F_ID,F_PAYMENT_TYPE_NO,F_ORDER_NO,F_ORDER_YEAR,F_ORDER_DATE,F_MEMBER" +
                          ",F_PENALTY_VALUE,F_SEND_COST,F_VALUE,F_TOTAL_VALUE,F_PAYMENT_METHOD_ID,F_CARD,F_PAY_IMAGE)" +
                          " VALUES (:p,:p0,:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11)";

                var entity = await _context.Database.ExecuteSqlRawAsync(sql,
                    new OracleParameter("p", model.F_ID),
                    new OracleParameter("p0", model.F_PAYMENT_TYPE_NO), //).PaymentMemCard?.F_DELIVERY_METHOD),
                    new OracleParameter("p1", model.F_ORDER_NO),                   
                    new OracleParameter("p2", model.F_ORDER_YEAR),
                    new OracleParameter("p3", model.F_ORDER_DATE),
                    new OracleParameter("p4", model.F_MEMBER),
                    new OracleParameter("p5", model.F_PENALTY_VALUE),
                    new OracleParameter("p6", model.F_SEND_COST),
                    new OracleParameter("p7", model.F_VALUE),
                    new OracleParameter("p8", model.F_TOTAL_VALUE),
                    new OracleParameter("p9", model.F_PAYMENT_METHOD_ID),
                    new OracleParameter("p10", model.F_CARD) ,
                    new OracleParameter("p11", model.F_PAY_IMAGE)
                    );

                await _context.SaveChangesAsync();

                short ser = 0;
                // Retrieve the selected values from TempData
                var SelectedValuesList = JsonConvert.DeserializeObject<List<SelectedValue>>(TempData["SelectedValues"]?.ToString());

                if (SelectedValuesList != null)
                {
                    foreach (var item in SelectedValuesList)
                    {
                        ser++;
                        //var sqlDet = "INSERT INTO T_PAYMENT_DETAIL (F_ID,F_ID_MAIN,F_SER,F_PAY_YEAR,F_PAY_MONTH,F_PAY_VALUE)" +
                        //   " VALUES (:p0,:p1,:p2,:p3,:p4,:p5)";

                        var sqlDet = "INSERT INTO T_PAYMENT_DETAIL (F_SER,F_MAIN_ID,F_PAY_YEAR,F_PAY_MONTH,F_PAY_VALUE)" +
                           " VALUES (:p1,:p2,:p3,:p4,:p5)";

                        var entityDet = await _context.Database.ExecuteSqlRawAsync(sqlDet,
                            // new OracleParameter("p0", model.F_ID),
                            new OracleParameter("p1", ser),
                            new OracleParameter("p2", model.F_ID),                            
                            new OracleParameter("p3", item.F_YEAR),
                            new OracleParameter("p4", 12),
                            new OracleParameter("p5", item.VALUE)
                            );
                    }
                }

                await _context.SaveChangesAsync();


                if (model.PaymentMemCard != null)
                {                    
                    var sqlMemCard = "INSERT INTO T_PAYMENT_MEM_CARD (F_BRANCHNO,F_PAYMENT_METHOD,F_MEM_NO,F_SEND_COST,F_MEM_ADDRESS,F_SEND_DATE)" +
                        " VALUES (:p1,:p2,:p3,:p4,:p5,:p6)";

                    var entityDet = await _context.Database.ExecuteSqlRawAsync(sqlMemCard,
                        // new OracleParameter("p0", model.F_ID),
                        new OracleParameter("p1", model.PaymentMemCard.F_BRANCHNO),
                        new OracleParameter("p2", model.F_PAYMENT_METHOD_ID),
                        new OracleParameter("p3", model.F_MEMBER),
                        new OracleParameter("p4", model.PaymentMemCard.F_SEND_COST),
                        new OracleParameter("p5", model.PaymentMemCard.F_MEM_ADDRESS),
                        new OracleParameter("p6", DateTime.Now)
                       // new OracleParameter("p7", 0)
                        );                   
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = model.F_ID });
                }

                //return View();
                return RedirectToAction("Index"); // or wherever you want
            }
            catch (Exception ex)
            {
                //return View();
                // Option 1: show the error in the same view
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("SendMemCard", model);
            }
            
        }


        private GetObject GetValues(List<int> selectedItems)
        {            

            decimal totalValues = 0;

            // Retrieve the list of selected item values from the form
            // var selectedItems = Request.Form["selectedItems"].ToList();

            //    List<(int F_YEAR, decimal VALUE)> selectedValues = TempData["SelectedValues"] as List<(int F_YEAR, decimal VALUE)>
            //?? new List<(int F_YEAR, decimal VALUE)>();


            // Retrieve existing selected values from TempData or initialize a new list
            var selectedValues = new List<SelectedValue>();

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

                            // Create a new SelectedValue instance
                            var newSelectedValue = new SelectedValue
                            {
                                F_YEAR = fYear,
                                VALUE = decimalValue
                            };

                            // Add the new value to the list
                            selectedValues.Add(newSelectedValue);

                        }
                    }
                }
            }

            // You can now use the selectedValues list in your logic
            // For example, passing it to a view or further processing
            //return View(selectedValues);
            // Save it back to session
            //HttpContext.Session.SetObjectAsJson(SelectedValuesSessionKey, SelectedValues);           

            return new GetObject { TotalValues = totalValues , SelectedValues = selectedValues };
                
          //  return View();
        }

        // GET: T_PAYMENT_MAIN/Edit/5
        public async Task<IActionResult> SendMemCard(decimal totalValue, decimal F_VALUE)
        {
            //var branchs = _context.V_T_GOV
            //    .Select(c => new SelectListItem
            //    {
            //        Value = c.F_CODE.ToString(),
            //        Text = c.F_NAME
            //    }).ToList();

            //// Add an initial empty option to the list
            //branchs.Insert(0, new SelectListItem
            //{
            //    Value = "", // This will result in a null or empty value in the model
            //    Text = "اختار المحافظة" // Placeholder text
            //});

            //ViewBag.BranchsList = branchs;
            //// ==========================================
            var methodsList = _context.V_SYSCODE.Where(e => e.F_TYPE == 27)
              .Select(c => new SelectListItem
              {
                  Value = c.F_CODE.ToString(),
                  Text = c.F_NAME
              }).ToList();

            methodsList.Insert(0, new SelectListItem
            {
                Value = "", // This will result in a null or empty value in the model
                Text = "اختار طرق الدفع" // Placeholder text
            });

            ViewBag.MethodsList = methodsList;
            //// ==========================================
            var deliveryList = _context.T_PAYMENT_METHOD.Where(e => e.F_IS_ACTIVE == 1)
                .Select(c => new SelectListItem
                {
                    Value = c.F_ID.ToString(),
                    Text = c.F_NAME
                }).ToList();

            // Add an initial empty option to the list
            deliveryList.Insert(0, new SelectListItem
            {
                Value = "", // This will result in a null or empty value in the model
                Text = "اختار طرق التوصيل" // Placeholder text
            });
           
            ViewBag.DeliveryList = deliveryList;

            //var paymentMemCard = new T_PAYMENT_MEM_CARD
            //{
            //    F_TOTAL_VALUE =  cardValue,
            //    F_MEM_No = DateTime.Now.Year, // Set the default year to the current year
            //    F_MEM_ADDRESS = DateTime.Now,
            //    F_PAYMENT_METHOD = "البنك الاهلي المصري – الفرع الرئيسي",              
            //};

            var model = new T_PAYMENT_MAIN
            {
                F_VALUE = F_VALUE,
                F_TOTAL_VALUE = totalValue,
                F_PENALTY_VALUE = Convert.ToDecimal(HttpContext.Session.GetString("PenaltyValue")),
                F_ORDER_YEAR = DateTime.Now.Year, // Set the default year to the current year
                F_ORDER_DATE = DateTime.Now,
                //F_BANK_NAME = "البنك الاهلي المصري – الفرع الرئيسي",
                //F_CARD = "0013070556722601012",
                //PaymentMemCard = paymentMemCard
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult GetSendCost(int methodId, int branchId)
        {
            // Fetch the cost based on the selected method ID and branch ID
            decimal sendCost = 0;
            try
            {
                if (methodId == 2)
                {
                    var sendCostList = _context.V_T_GOV
                   .Where(e => e.F_CODE == branchId)
                   .Select(e => e.F_SEND_COST)
                   .ToList();

                    if (sendCostList != null)
                        sendCost = sendCostList[0];
                }
                else
                {
                    var sendCostList = _context.V_T_BRANCH_DESCRIPTION
                  .Where(e => e.F_BRANCHNO == branchId)
                  .Select(e => e.F_SEND_COST)
                  .ToList();

                    if (sendCostList != null)
                        sendCost = sendCostList[0];
                }

                // Return the send cost as JSON
                return Json(new { sendCost });
            }
            catch (Exception ex)
            {
                return Json(new { sendCost = 100 });
               // throw ex;
            }
           
        }


        [HttpGet]
        public IActionResult GetSendCostxxx(string methodId, string branchId)
        {
            // Fetch the cost based on the selected method ID and branch ID
            decimal sendCost = 0;

            if (methodId == "1")
            {
                sendCost = _context.V_T_BRANCH_DESCRIPTION
               .Where(e => e.F_BRANCHNO == Convert.ToInt16(branchId))
               .Select(pc => pc.F_SEND_COST)
               .FirstOrDefault();
            }

            // Return the send cost as JSON
            return Json(new { sendCost });
        }

        #endregion


        // GET: T_PAYMENT_MAIN/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var T_PAYMENT_MAIN = await _context.T_PAYMENT_MAIN.Where(e => e.F_ID == id).ToListAsync();

            if (T_PAYMENT_MAIN[0] == null)
            {
                return NotFound();
            }

            return View(T_PAYMENT_MAIN[0]);
        }

        // POST: T_PAYMENT_MAIN/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("F_ORDER_NO,F_COMPANY_NO,F_ORDER_YEAR,F_MEMBER,F_TOTAL_VALUE,F_REFERENCE_NO,F_PAYMENT,F_HAFZA_NO,F_HAFZA_YEAR,F_BANK_NAME,F_CARD,F_ORDER_DATE,F_PAY_FEES,F_POST_COST,F_PENALTY")] T_PAYMENT model)
        {
            if (id != model.F_ORDER_NO)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // _context.Update(T_PAYMENT_MAIN);
                    // Example of using raw SQL
                    var sql = "Update T_PAYMENT_MAIN Set (F_ORDER_NO:p1,F_COMPANY_NO:p2, F_ORDER_YEAR:p3,F_MEMBER:p4,F_TOTAL_VALUE:p5,F_REFERENCE_NO:p6,F_PAYMENT:p7,F_HAFZA_NO:p8,F_HAFZA_YEAR:p9,F_CARD:p11,F_ORDER_DATE:p12,F_PAY_FEES:p13,F_POST_COST:p14,F_PENALTY:p15) Where F_ID = :p0";
                    await _context.Database.ExecuteSqlRawAsync(sql,
                        new OracleParameter("p0", model.F_ID),
                        new OracleParameter("p1", model.F_ORDER_NO),
                        new OracleParameter("p2", model.F_COMPANY_NO),
                        new OracleParameter("p3", model.F_ORDER_YEAR),
                        new OracleParameter("p4", model.F_MEMBER),
                        new OracleParameter("p5", model.F_TOTAL_VALUE),
                        new OracleParameter("p6", model.F_REFERENCE_NO),
                        new OracleParameter("p7", model.F_PAYMENT),
                        new OracleParameter("p8", model.F_HAFZA_NO),
                        new OracleParameter("p9", model.F_HAFZA_YEAR),                       
                        //new OracleParameter("p10", model.F_BANK_NAME),
                        new OracleParameter("p11", model.F_CARD),
                        new OracleParameter("p12", model.F_ORDER_DATE),
                        new OracleParameter("p13", model.F_PAY_FEES),
                        new OracleParameter("p14", model.F_POST_COST),
                        new OracleParameter("p15", model.F_PENALTY));
                   
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!T_PAYMENT_MAINExists(model.F_ORDER_NO))
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
            return View(model);
        }

        // GET: T_PAYMENT_MAIN/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var T_PAYMENT_MAIN = await _context.T_PAYMENT_MAIN
            //    .FirstOrDefaultAsync(m => m.F_ORDER_NO == id);
            //if (T_PAYMENT_MAIN == null)
            //{
            //    return NotFound();
            //}

            return View(); // View(T_PAYMENT_MAIN);
        }

        // POST: T_PAYMENT_MAIN/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _context.T_PAYMENT_MAIN
                                           .Where(e => e.F_ID == id)
                                           .Select(n => new T_PAYMENT_MAIN
                                           {
                                               F_ID = n.F_ID,
                                               F_TOTAL_VALUE = n.F_TOTAL_VALUE,
                                               F_PAYMENT_METHOD_ID = n.F_PAYMENT_METHOD_ID,
                                               F_CARD = n.F_CARD,
                                               F_ORDER_NO = n.F_ORDER_NO,
                                               F_ORDER_YEAR = n.F_ORDER_YEAR,
                                               F_ORDER_DATE = n.F_ORDER_DATE,
                                           })
                                           .ToListAsync(); //.FindAsync(id);
            if (result != null)
            {
                foreach (var item in result)
                {
                    _context.T_PAYMENT_MAIN.Remove(item);
                }
                
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool T_PAYMENT_MAINExists(int id)
        {
            return _context.T_PAYMENT_MAIN.Any(e => e.F_ORDER_NO == id);
        }


        [HttpGet]
        public JsonResult GetBranchesByMethod(int paymentMethod)
        {
            // Fetch branches based on selected payment method
            if (paymentMethod == 2)
            {
                var gov = _context.V_T_GOV
               .Select(b => new SelectListItem
               {
                   Value = b.F_CODE.ToString(),
                   Text = b.F_NAME
               }).ToList();

                // Add an initial empty option to the list
                gov.Insert(0, new SelectListItem
                {
                    Value = "", // This will result in a null or empty value in the model
                    Text = "اختار المحافظة" // Placeholder text
                });

                return Json(gov);
            }

            var branches = _context.V_T_BRANCH_DESCRIPTION
                .Select(b => new SelectListItem
                {
                    Value = b.F_BRANCHNO.ToString(),
                    Text = b.F_TITTLE
                }).ToList();

            // Add an initial empty option to the list
            branches.Insert(0, new SelectListItem
            {
                Value = "", // This will result in a null or empty value in the model
                Text = "اختار الفرع" // Placeholder text
            });

            return Json(branches);
        }


        // GET: T_PAYMENT_MAIN/Edit/5
        public IActionResult GetBranchesByMethodxxx(decimal totalValue, decimal F_VALUE)
        {
            var branchs = _context.V_T_GOV
                .Select(c => new SelectListItem
                {
                    Value = c.F_CODE.ToString(),
                    Text = c.F_NAME
                }).ToList();

            // Add an initial empty option to the list
            branchs.Insert(0, new SelectListItem
            {
                Value = "", // This will result in a null or empty value in the model
                Text = "اختار المحافظة" // Placeholder text
            });

            ViewBag.BranchsList = branchs;
            if (branchs != null)
            {
                return Json(new { success = true, memberName = branchs });
            }
            return Json(new { success = false });
        }

        public async Task<IActionResult> PaymentReport(int? DeliveryType, int? PaymentMethod, DateTime? startDate, DateTime? endDate)
        {
            List<PaymentMainDto> results = new List<PaymentMainDto>();

            try
            {

                //// ==========================================
                var methodsList = _context.V_SYSCODE.Where(e => e.F_TYPE == 27)
                  .Select(c => new SelectListItem
                  {
                      Value = c.F_CODE.ToString(),
                      Text = c.F_NAME
                  }).ToList();

                methodsList.Insert(0, new SelectListItem
                {
                    Value = "", // This will result in a null or empty value in the model
                    Text = "اختار طرق الدفع" // Placeholder text
                });

                ViewBag.PaymentMethods = methodsList;
                //// ==========================================
                var deliveryTypesList = _context.V_SYSCODE.Where(e => e.F_TYPE == 28)
                  .Select(c => new SelectListItem
                  {
                      Value = c.F_CODE.ToString(),
                      Text = c.F_NAME
                  }).ToList();

                deliveryTypesList.Insert(0, new SelectListItem
                {
                    Value = "", // This will result in a null or empty value in the model
                    Text = "اختار طرق التوصيل" // Placeholder text
                });

                ViewBag.DeliveryTypes = deliveryTypesList;
                //// ==========================================

                //ViewBag.PaymentTypes = new SelectList(
                //                       await _context.V_SYSCODE
                //                      .Where(t => t.F_TYPE == 28)
                //                           .Select(e => new
                //                           {
                //                               Value = e.F_CODE,
                //                               Text = e.F_NAME
                //                           })
                //                            .ToListAsync(),
                //                        "Value",
                //                        "Text"
                //                    );                

                var data = _context.V_PAYMENT_MAIN.AsQueryable();

                if (DeliveryType > 0)
                    data = data.Where(p => p.F_PAYMENT_TYPE_NO == DeliveryType);

                if (PaymentMethod > 0)
                    data = data.Where(p => p.F_PAYMENT_METHOD_ID == PaymentMethod);

                if (startDate.HasValue && endDate.HasValue)
                    data = data.Where(p => p.F_ORDER_DATE >= startDate.Value && p.F_ORDER_DATE <= endDate.Value);

                results = await data
                        .Select(e => new PaymentMainDto
                        {
                            F_ID = e.F_ID,
                            F_ORDER_NO = e.F_ORDER_NO,
                            F_ORDER_DATE = e.F_ORDER_DATE,
                            F_DELIVERY_METHOD = e.F_DELIVERY_METHOD,
                            F_PAYMENT_METHOD = e.F_PAYMENT_METHOD,
                            F_MEMBER = e.F_MEMBER,
                            F_MEMBER_NAME = e.F_MEMBER_NAME,
                            F_PENALTY_VALUE = e.F_PENALTY_VALUE,
                            F_TOTAL_VALUE = e.F_TOTAL_VALUE
                        })
                        .OrderBy(d=>d.F_ORDER_NO)
                        .ToListAsync();         

                return View(results);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a single payment record's image data by its ID.
        /// This is a separate method to avoid loading large binary data unnecessarily.
        /// </summary>
        /// <param name="id">The unique identifier of the payment record.</param>
        /// <returns>A byte array containing the image data, or null if not found.</returns>
              
        public async Task<IActionResult> GetPaymentImage(int id)
        {
            try
            {
                var data = _context.T_PAYMENT_MAIN
                         .Where(e => e.F_ID == id)
                         .ToList() // This executes the query and brings all data into memory
                         .Select(e => new
                         {
                             F_ID = e.F_ID,
                             F_PAY_IMAGE = e.F_PAY_IMAGE
                         });

                if (data == null)
            {
                // Return a 404 Not Found or a placeholder image if no image exists.
                return NotFound();
            }

                var imageData = data.First().F_PAY_IMAGE;

                if (imageData == null) {
                    return NotFound();
                }
            // You must specify the correct image type.
            // For a JPEG image:
            return File(imageData, "image/jpeg");

                // For a PNG image:
                // return File(imageData, "image/png");
            }
            catch ( Exception ex) {
               //return {  ex.Message()};
                throw ex;
            }
           
        }
        public async Task<IActionResult> SendMemCardReport(int? memCode, int? DeliveryType, int? PaymentMethod, DateTime? startDate, DateTime? endDate)
        {
            PaymentMainWithDetailsDto results = new();

            if (memCode == null)
                return View(results);          

            try
            {
                //// ==========================================
                var methodsList = _context.V_SYSCODE.Where(e => e.F_TYPE == 27)
                  .Select(c => new SelectListItem
                  {
                      Value = c.F_CODE.ToString(),
                      Text = c.F_NAME
                  }).ToList();

                methodsList.Insert(0, new SelectListItem
                {
                    Value = "", // This will result in a null or empty value in the model
                    Text = "اختار طرق الدفع" // Placeholder text
                });

                ViewBag.PaymentMethods = methodsList;
                //// ==========================================
                var deliveryTypesList = _context.V_SYSCODE.Where(e => e.F_TYPE == 28)
                  .Select(c => new SelectListItem
                  {
                      Value = c.F_CODE.ToString(),
                      Text = c.F_NAME
                  }).ToList();

                deliveryTypesList.Insert(0, new SelectListItem
                {
                    Value = "", // This will result in a null or empty value in the model
                    Text = "اختار طرق التوصيل" // Placeholder text
                });

                ViewBag.DeliveryTypes = deliveryTypesList;
                //// ==========================================

                //ViewBag.PaymentTypes = new SelectList(
                //                       await _context.V_SYSCODE
                //                      .Where(t => t.F_TYPE == 28)
                //                           .Select(e => new
                //                           {
                //                               Value = e.F_CODE,
                //                               Text = e.F_NAME
                //                           })
                //                            .ToListAsync(),
                //                        "Value",
                //                        "Text"
                //                    );                

                var data = _context.V_PAYMENT_MAIN.AsQueryable();

                if (memCode > 0)
                    data = data.Where(p => p.F_MEMBER == memCode);

                if (DeliveryType > 0)
                    data = data.Where(p => p.F_PAYMENT_TYPE_NO == DeliveryType);
                
                if (PaymentMethod > 0)
                    data = data.Where(p => p.F_PAYMENT_METHOD_ID == PaymentMethod);

                if (startDate.HasValue && endDate.HasValue)
                    data = data.Where(p => p.F_ORDER_DATE >= startDate.Value && p.F_ORDER_DATE <= endDate.Value);

                //// Step 1: pull data into memory safely (after filtering)
                //var query = await data
                //    .Where(e => e.F_MEMBER == memCode)
                //     .Select(e => new PaymentMainWithDetailsDto
                //     {
                //         F_ID = e.F_ID,
                //         F_ORDER_NO = e.F_ORDER_NO,
                //         F_ORDER_DATE = e.F_ORDER_DATE,
                //         F_DELIVERY_METHOD = e.F_DELIVERY_METHOD,
                //         F_PAYMENT_METHOD = e.F_PAYMENT_METHOD,
                //         F_MEMBER = e.F_MEMBER,
                //         F_MEMBER_NAME = e.F_MEMBER_NAME,
                //         F_PENALTY_VALUE = e.F_PENALTY_VALUE,
                //         F_TOTAL_VALUE = e.F_TOTAL_VALUE
                //     })
                //     .Take(1)
                //    .ToListAsync();  

                //if (query == null)
                //    return View(results);

                var query = data
                         .Select(e => new PaymentMainWithDetailsDto
                         {
                             F_ID = e.F_ID,
                             F_ORDER_NO = e.F_ORDER_NO,
                             F_ORDER_DATE = e.F_ORDER_DATE,
                             F_DELIVERY_METHOD = e.F_DELIVERY_METHOD,
                             F_PAYMENT_METHOD = e.F_PAYMENT_METHOD,
                             F_MEMBER = e.F_MEMBER,
                             F_MEMBER_NAME = e.F_MEMBER_NAME,
                             F_PENALTY_VALUE = e.F_PENALTY_VALUE,
                             F_TOTAL_VALUE = e.F_TOTAL_VALUE
                         })
                         .ToList();

                if (query.Count == 0)
                    return View(results);
                var mainId = query[0].F_ID;

                query[0].PaymentDetails = _context.T_PAYMENT_DETAIL
                                        .Where(p => p.F_MAIN_ID == mainId)
                                        .Select(e => new PaymentDetailDto
                                        {
                                            F_SER = e.F_SER,
                                            F_PAY_YEAR = e.F_PAY_YEAR,
                                            F_PAY_VALUE = e.F_PAY_VALUE,
                                            F_DESC_PAY = e.F_DESC_PAY
                                        })
                                        .ToList();

                return View(query[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    internal class GetObject : ReturnObject
    {
        public decimal TotalValues { get; set; }
        public List<SelectedValue> SelectedValues { get; set; }
    }

    class ReturnObject
    {
        public decimal TotalValues { get; set; }
        public List<(int F_YEAR, decimal VALUE)> SelectedValues { get; set; }
    }

    [Serializable] // This is optional but helps signify that the class is meant for serialization.
    public class SelectedValue
    {
        public int F_YEAR { get; set; }
        public decimal VALUE { get; set; }
    }

    [Serializable] // This is optional but helps signify that the class is meant for serialization.
    public class MethodList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
