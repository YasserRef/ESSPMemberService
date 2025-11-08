using System;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ESSPMemberService.Data;
using Microsoft.AspNetCore.Mvc;
using BotDetect.Web.Mvc;

namespace ESSPMemberService.Controllers
{
    public class MemDescController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemDescController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}
            HttpContext.Session.SetString("MemID", "");
            ViewBag.ErrorMsg = "";

            return View();
        }

        public ActionResult Login()
        {
            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}
            HttpContext.Session.SetString("MemID", "");
            ViewBag.ErrorMsg = "";

            return View();
        }

        //public ActionResult Index(MemDes model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ViewBag.Name = model.F_CODE;
        //        ViewBag.Email = model.F_BRANCHNO;
        //    }
        //    return View(model);
        //}
       
       
        // private CaptchaValidation("CaptchaCode", "MemDes", "الكود غير مطابق للصورة!")]
        [HttpPost]
        public ActionResult Index(int MemID = 0, int ClassID = 0)
        {

            //Session["MemID"] = "";
            // Set a session value
            HttpContext.Session.SetString("MemID", "");

            ViewBag.ErrorMsg = "";

            if (!ModelState.IsValid)
            {
                MvcCaptcha.ResetCaptcha("MemDes");
                return View();
            }

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

            if (ClassID <= 0 )
            {
                ViewBag.ErrorMsg += "يجب ادخال رقم الشعبة للعضو";
                return View();
            }

            var Result = _context.V_MEMBER_INFO.Where(m => m.F_CODE == MemID && m.F_BRANCHNO == ClassID).Single();

            //var Result = from m in data
            //             select m;

            //if (MemID>0 && !String.IsNullOrEmpty(ClassID))
            //{
            //    Result = Result.Where(s => s.F_CODE == MemID);
            //    Result = Result.Where(s => s.NOTES == ClassID);
            //}


            if (Result != null)
            {
                HttpContext.Session.SetString("MemID", MemID.ToString());
                HttpContext.Session.SetString("MemName", Result.F_NAME);
                HttpContext.Session.SetString("LatePay", Result.F_LATEPAY.ToString());
                HttpContext.Session.SetString("PaYear", Result.F_PAYEAR.ToString());
                HttpContext.Session.SetString("MainWrkSide", Result.F_MAINWRKSIDE.ToString()); 

                return RedirectToAction("Main");
            }
            else
            {
                ViewBag.ErrorMsg = "تسجيل الدخول غير سليم";
                return View(); // return RedirectToAction("Index", "MemDes");
            }
        }

        public ActionResult Main()
        {
            ViewBag.Message = "Your main page.";

            return View();
        }

        //[HttpPost]
        //public ActionResult Index(MemSrv.External.Models.MemDes model)
        //{
        //    if (model.F_CODE <=0)
        //    {
        //        ModelState.AddModelError("F_CODE", "يجب ادخال رقم القيد");
        //    }

        //    if (string.IsNullOrEmpty(model.F_BRANCHNO))
        //    {
        //        ModelState.AddModelError("F_BRANCHNO", "يجب ادخال رقم الشعبة");
        //    }

        /* 
          if (!string.IsNullOrEmpty(model.Email))
          {
              string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                       @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                          @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
              Regex re = new Regex(emailRegex);
              if (!re.IsMatch(model.Email))
              {
                  ModelState.AddModelError("Email", "Email is not valid");
              }
          }
          else
          {
              ModelState.AddModelError("Email", "Email is required");
          }

          */

        //    if (ModelState.IsValid)
        //    {
        //        ViewBag.Name = model.F_CODE;
        //        ViewBag.Email = model.F_BRANCHNO;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    return View(model);
        //}

        //
        // POST: /Product/Create

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Index([Bind(Exclude = "Id")] MemDes MemDes)
        //{
        //    // Validation logic
        //    if (MemDes.F_CODE.ToString().Trim().Length == 0)
        //        ModelState.AddModelError("Name", "F_CODE is required.");
        //    if (MemDes.F_BRANCHNO.Trim().Length == 0)
        //        ModelState.AddModelError("Description", "F_BRANCHNO is required.");
        //    return View();
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactSubmit(
                [Bind("F_CODE, F_NAME, F_NATIONNO, F_BRANCHNO")]   V_DIFFMEMBER MemDes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //MemDes.F_CODE = SessionIDManager.Get<F_CODE>(Constants.SessionTenant);
                    //MemDes.F_BRANCHNO = SessionManager.Get<string>(Constants.SessionRefferal);
                    //DataStoreManager.AddMemDes(MemDes);
                    return RedirectToAction("SubmissionConfirmed", MemDes);
                }
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("", "Unable to perform action. Please contact us.");
                return RedirectToAction("SubmissionFailed", MemDes);
            }

            return View(MemDes);
        }

        [HttpGet]
        public ActionResult ContactSubmit()
        {
            return View();
        }


        // GET: SpendPension
        //public ActionResult Index(string MemID, string ClassID)
        //{
        //    XMLReaderMemDes readXMLPension = new XMLReaderMemDes();
        //    var data = readXMLPension.RetrunListOfMemDes();

        //    var Result = from m in data
        //                 select m;

        //    Session["MemID"] = "";

        //    if (!String.IsNullOrEmpty(MemID) && !String.IsNullOrEmpty(ClassID))
        //    {              
        //        Result = data.ToList().Where(s => s.F_CODE.Equals(MemID) );
        //        Result = data.ToList().Where(s => s.F_BRANCHNO.Contains(ClassID));
        //    }

        //    if (Result != null)
        //    {
        //        Session["MemID"] = MemID;
        //        return View(Result);
        //    }
        //    else
        //    { 
        //        return View();
        //    }


        //    // string searchString = "111";
        //    // data = data.Where(s => s.MEM_NAME.Contains(searchString));
        //    //1310, 59245, 59573

        //    //return View(data.ToList().Where(s => s.F_CODE.Equals(Session["MemID"])));

        //    /*
        //    //Now set up the config xml read
        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.Load(HttpContext.Server.MapPath("~/App_Data/SPENDDATE_Self.xml"));
        //    XmlNodeList settings = xmldoc.SelectNodes("/DocumentElement");
        //    XmlNodeList defaults = xmldoc.GetElementsByTagName("SPENDDATE_Self");
        //    foreach (XmlNode node in defaults)
        //    {
        //        string def_WebPageName = node["SPENDDATE_Self"].InnerText;
        //    }

        //    return View();
        //    */

        //}


        //// [ValidateAntiForgeryToken]
        //[Route("API/DiffMem/PostPay")]
        //public JsonResult PostPay(fawrypay_request PayRequest)
        [HttpPost]  //[HttpGet]
        // [ValidateAntiForgeryToken]
        // [Route("API/DiffMem/PostPay")]
        public JsonResult PostPay1(fawrypay_request1 PayRequest)
        {
            //Merchant code: 1tSa6uxz2nSoArPUo4gkcg==
            //Security key: 4ebea01f72a3487baff7e2c3f5cc55c8
            //Hash Key: 4ebea01f72a3487baff7e2c3f5cc55c8

            PayRequest = new fawrypay_request1
            {
                merchantCode = "1tSa6uxz2nSoArPUo4gkcg==",
                merchantRefNum = "99900642041",
                customerName = "Yasser Mohamed",
                customerMobile = "01234567891",
                customerEmail = "example@gmail.com",
                customerProfileId = "777777",
                amount = "10",
                currencyCode = "EGP",
                language = "en-gb",
                chargeItems = new ChargeItems
                {
                    itemId = "897fa8e81be26df25db592e81c31c",
                    description = "Item Description",
                    price = "580.55",
                    quantity = "1"
                },
                signature = "059dad78530b15f10bb86d971687d1b3466cfa2fe3366f2fdbf8f17d2f06ab63",
                payment_method = "MWALLET",
                description = "example description"
            };

            // For hashing a correct signature you need to hash it  by the SHA - 256 of  
            // merchantCode + merchantRefNum + customerProfileId + returnUrl + Item ID + price(Decimal with 2 fraction parts) + secure_key

            PostJson1("https://atfawry.fawrystaging.com/ECommerceWeb/api/payments/charge", PayRequest);
        
            //TODO: user now contains the details, you can do required operations  
            return Json("new fawrypay request send");
        }

      
        public JsonResult PostPay(fawrypay_request PayRequest)
        {
            fawrypay_request Fawrypay_request = new fawrypay_request
            {
                merchantCode = "1tSa6uxz2nSoArPUo4gkcg==",
                merchantRefNumber = "77118899",
                signature = "059dad78530b15f10bb86d971687d1b3466cfa2fe3366f2fdbf8f17d2f06ab63"
            };

          PostJson("https://atfawry.fawrystaging.com/ECommerceWeb/Fawry/payments/status", Fawrypay_request);

            return Json("new fawrypay request send");
        }

        private static void PostJson(string uri, fawrypay_request postParameters)
        {
            string postData = JsonConvert.SerializeObject(postParameters);
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.ContentType = "text/json";
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Count());
            }
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                string message = String.Format("GET failed. Received HTTP {0}", httpWebResponse.StatusCode);
                throw new ApplicationException(message);
            }
        }
    

        private static void PostJson1(string uri, fawrypay_request1 postParameters)
        {
            string postData = JsonConvert.SerializeObject(postParameters);
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.ContentType = "text/json";
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Count());
            }
            var httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                string message = String.Format("GET failed. Received HTTP {0}", httpWebResponse.StatusCode);
                throw new ApplicationException(message);
            }
        }

    }



    public class fawrypay_request
    {
        public string merchantCode { get; set; }
        public string merchantRefNumber { get; set; }
        public string signature { get; set; }
    }

    public class fawrypay_request1
    {
        public string merchantCode { get; set; }
        public string merchantRefNum { get; set; }
        public string signature { get; set; }
        //public string merchantCode { get; set; }
        //public string merchantRefNum { get; set; }
        public string customerName { get; set; }
        public string customerMobile { get; set; }
        public string customerEmail { get; set; }
        public string customerProfileId { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string language { get; set; }
        public ChargeItems chargeItems;
        // public string signature { get; set; }
        public string payment_method { get; set; }
        public string description { get; set; }

    }

    public class ChargeItems
    {
        public string itemId { get; set; }
        public string description { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
    }


}
