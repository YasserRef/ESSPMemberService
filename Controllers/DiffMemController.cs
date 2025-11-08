using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using ESSPMemberService.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ESSPMemberService.Controllers
{
    public class DiffMemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiffMemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiffMem
        public ActionResult Index()
        {
            try { 
                var MemId = Convert.ToInt64(HttpContext.Session.GetString("MemID"));

                if (MemId <= 0)
                    return RedirectToAction("Index", "MemDes");

                decimal total = 0;
                var Result = _context.V_DIFFMEMBER.Where(m => m.F_CODE == MemId).ToList();

                // Add teh current year
                // F_VALUE + F_SERVICE_BOX + F_ESSP_CARD + تغمة الايصال (10 جنيه)
                // = 100+50+50+10 = 210
                //
                int currentYear = DateTime.Now.Year;

                var LatePay = HttpContext.Session.GetInt32("LatePay");
                var PaYear = HttpContext.Session.GetInt32("PaYear");
                var MainWrkSide = HttpContext.Session.GetInt32("MainWrkSide");

                if ( MainWrkSide != 500 && MainWrkSide != 550)
                {
                    if (LatePay >= 0 && PaYear < currentYear)
                    {
                        var newDiffMember = new V_DIFFMEMBER
                        {
                            F_YEAR = Convert.ToInt16(currentYear),
                            VALUE = 210
                        };
                        Result.Add(newDiffMember);
                    }
                  
                }
                


                foreach (var item in Result)
                {
                    total = total + item.VALUE.Value;
                }

                ViewBag.Total_Value = total;
                return View(Result);
            }

            catch
            {
                return RedirectToAction("Index", "MemDes");
            }

        }

        // GET: DiffMem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            V_DIFFMEMBER model = _context.V_DIFFMEMBER.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: DiffMem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DiffMem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "F_CODE,F_YEAR,DATE_DIFF,VALUE,NOTES,F_ISPAY_PRCENT")] V_DIFFMEMBER v_T_DIFFMEMBER)
        {
            if (ModelState.IsValid)
            {
                _context.V_DIFFMEMBER.Add(v_T_DIFFMEMBER);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(v_T_DIFFMEMBER);
        }

        // GET: DiffMem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            V_DIFFMEMBER v_T_DIFFMEMBER = _context.V_DIFFMEMBER.Find(id);
            if (v_T_DIFFMEMBER == null)
            {
                return HttpNotFound();
            }
            return View(v_T_DIFFMEMBER);
        }

        // POST: DiffMem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "F_CODE,F_YEAR,DATE_DIFF,VALUE,NOTES,F_ISPAY_PRCENT")] V_DIFFMEMBER v_T_DIFFMEMBER)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(v_T_DIFFMEMBER).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(v_T_DIFFMEMBER);
        }

        // GET: DiffMem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            V_DIFFMEMBER v_T_DIFFMEMBER = _context.V_DIFFMEMBER.Find(id);
            if (v_T_DIFFMEMBER == null)
            {
                return HttpNotFound();
            }
            return View(v_T_DIFFMEMBER);
        }

        // POST: DiffMem/Delete/5
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            V_DIFFMEMBER v_T_DIFFMEMBER = _context.V_DIFFMEMBER.Find(id);
            _context.V_DIFFMEMBER.Remove(v_T_DIFFMEMBER);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }


        //==================================================================
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public ActionResult PostPay()
        {
            //For hashing a correct signature you need to hash it  by the SHA - 256 of 
            //merchantCode +merchantRefNum + customerProfileId + returnUrl + Item ID + price(Decimal with 2 fraction parts) + secure_key
            
            //string hash = "";
            string merchantCode = "1tSa6uxz2nRlhbmxHHde5A==";
            string merchantRefNum = "99900642041";
            string returnUrl = "https://developer.fawrystaging.com";// "http://www.esspegypt.net:88/MemServices2021/DiffMem/PostPay";
            string ItemID = "897fa8e81be26df25db592e81c31c";
            double price = 10.00;
            string secure_key = "4ebea01f72a3487baff7e2c3f5cc55c8";
            string source = merchantCode + merchantRefNum + returnUrl + ItemID + price + secure_key;
            SHA256 sha256Hash = SHA256.Create();
            string signature = GetHash(sha256Hash, source);
           
            // Old One
            //PostJson("https://atfawry.fawrystaging.com/ECommerceWeb/Fawry/payments/status", new fawrypay_request
            //{
            //    merchantCode = "1tSa6uxz2nRlhbmxHHde5A==",
            //    merchantRefNum = "99900642041",
            //    customerName = "Ahmed Ali",
            //    customerMobile = "01234567891",
            //    customerEmail = "example@gmail.com",
            //    customerProfileId = "777777",
            //    amount = 580.55,
            //    //paymentExpiry : 1631138400000,
            //    currencyCode = "EGP",
            //    language = "en-gb",
            //    chargeItems = new ChargeItems  {
            //                       itemId =  "897fa8e81be26df25db592e81c31c",
            //                       description =  "Item Description",
            //                       price =  580.55,
            //                       quantity =  1
            //                     },
            //    signature = "3f527d0209f4fa5e370caf46f66597c6a7c04580c827ca1f29927ec0d9215131",
            //    payment_method = "PAYATFAWRY",
            //    description = "example description"
            //});

            object chargeRequest = new
            {
                merchantCode = "1tSa6uxz2nSoArPUo4gkcg==", // the merchant account number in Fawry
                merchantRefNum = "77118899", // order refrence number from merchant sidess
                customerMobile = "01555550229",
                customerEmail = "",
                customerName = "Mohammed",
                paymentExpiry = "1672351200000",
                customerProfileId = "1212", // in case merchant has customer profiling then can send profile id to attach it with order as reference 
                chargeItems = new
                {
                    itemId = "1222",
                    description = "1234",
                    price = 10,
                    quantity = 1,
                    imageUrl = "https://www.atfawry.com/ECommercePlugin/resources/images/atfawry-ar-logo.png"
                },
                selectedShippingAddress = new
                {
                    governorate = "", //Governorate code at Fawry
                    city = "", //City code at Fawry
                    area = "", //Area code at Fawry
                    address = "9th 90 Street, apartment number 8, 4th floor",
                    receiverName = "Mohamed"
                },
                paymentMethod = "",
                returnUrl = "https://developer.fawrystaging.com",
                signature = "059dad78530b15f10bb86d971687d1b3466cfa2fe3366f2fdbf8f17d2f06ab63"
            };

            PostJson("https://atfawry.fawrystaging.com/ECommerceWeb/Fawry/payments/status", chargeRequest);

            return Json("new fawrypay request send");
        }

        private static void PostJson(string uri, object postParameters)
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



        private object buildChargeRequest()
        {
            object chargeRequest = new  {
            merchantCode= "1tSa6uxz2nSoArPUo4gkcg==", // the merchant account number in Fawry
		    merchantRefNum= "77118899", // order refrence number from merchant sidess
		    customerMobile= "01555550229",
		    customerEmail= "",
		    customerName= "Mohammed",
		    paymentExpiry= "1672351200000",
		    customerProfileId= "1212", // in case merchant has customer profiling then can send profile id to attach it with order as reference 
		    chargeItems = new {
                    itemId= "1222",
				    description= "1234",
				    price= 10,
				    quantity= 1,
				    imageUrl= "https://www.atfawry.com/ECommercePlugin/resources/images/atfawry-ar-logo.png"
                },		
            selectedShippingAddress= new {
                    governorate= "", //Governorate code at Fawry
                    city= "", //City code at Fawry
                    area= "", //Area code at Fawry
                    address= "9th 90 Street, apartment number 8, 4th floor",
                    receiverName= "Mohamed"
                    },

		    paymentMethod= "",
		    returnUrl= "https://developer.fawrystaging.com",
		    signature= "059dad78530b15f10bb86d971687d1b3466cfa2fe3366f2fdbf8f17d2f06ab63"
        };
	
	return chargeRequest;
}

} // End Class

    public class fawrypay_request
    {
        public string merchantCode { get; set; }
        public string merchantRefNum { get; set; }       
        public string customerName { get; set; }
        public string customerMobile { get; set; }
        public string customerEmail { get; set; }
        public string customerProfileId { get; set; }
        public double amount { get; set; }
        //public string paymentExpiry { get; set; }
        public string currencyCode { get; set; }
        public string language { get; set; }
        public ChargeItems chargeItems;
        public string signature { get; set; }
        public string payment_method { get; set; }
        public string description { get; set; }

    }
    public class ChargeItems
    {
        public string itemId { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public decimal quantity { get; set; }
    }
    //=======================================================================

}
