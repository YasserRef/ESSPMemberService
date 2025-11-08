using System.Web.Mvc;
using System.Data;
using BotDetect.Web.Mvc;


namespace MemSrv.Controllers
{
    public class MemDesController : Controller
    {
        public ActionResult Index()
        {
            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}
            Session["MemID"] = "";
            ViewBag.ErrorMsg = "";
          
            return View();
        }


        public ActionResult FreeResort(string List_Resorts)
        {
           
            ResortNotRcrs readXML = new ResortNotRcrs();
            var data = readXML.RetrunListOfResortNotRcr();

            //=====================Fill List DropDownList========================='
            var ResortList = new List<string>();
            var ResortQuery = from q in data orderby q.HOLDAY_NAME select q.HOLDAY_NAME;
            ResortList.AddRange(ResortQuery.Distinct());
            ViewBag.List_Resorts = new SelectList(ResortList);
            //=============================================='
            var Result = from m in data
                         select m;

            if (!String.IsNullOrEmpty(List_Resorts))
            {
                 Result = Result.Where(s => s.HOLDAY_NAME == List_Resorts);
                return View(Result.ToList());
            }
            else
            {
                return View(data.ToList());

            }
                   

           

        }


        public ActionResult Login()
        {
            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Index");
            //}
            Session["MemID"] = "";
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

        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        [CaptchaValidation("CaptchaCode", "MemDes", "الكود غير مطابق للصورة!")]
        public ActionResult Index(int MemID=0 , string ClassID="")
        {
          
            Session["MemID"] = "";
            ViewBag.ErrorMsg = "";
          
             if(! ModelState.IsValid)
                {
                MvcCaptcha.ResetCaptcha("MemDes");
                return View();
            }

            if (MemID <= 0)
            {
                ViewBag.ErrorMsg = "يجب ادخال رقم القيد للعضو";
                return View();
            }

            if (String.IsNullOrEmpty(ClassID))
            {
                ViewBag.ErrorMsg +=  "يجب ادخال رقم الشعبة للعضو";
                return View();
            }

           ReaderMemDes readXML = new ReaderMemDes();
            var data = readXML.RetrunListOfMemDes();

            var Result = from m in data
                         select m;

            if (MemID>0 && !String.IsNullOrEmpty(ClassID))
            {
                Result = Result.Where(s => s.F_CODE == MemID);
                Result = Result.Where(s => s.F_BRANCHNO == ClassID);
            }

           
            if (Result.Count() > 0)
            {
                Session["MemID"] = MemID;
                Session["MemName"] = Result.ElementAt(0).F_NAME; //(from m in Result select m.F_NAME).ToString();
              
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
                [Bind(Include = "F_CODE, F_NAME, F_NATIONNO, F_BRANCHNO")]   MemDes MemDes)
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



    }
}