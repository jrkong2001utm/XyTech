using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Attributes;
using XyTech.Models;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class LandlordController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Landlord
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_indate.Day >= DateTime.Today.Day && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            

            var landlords = db.tb_landlord
                .Where(l => l.l_active != "0")
                .Include(l => l.tb_bankname)
                .OrderBy(l => l.l_due)
                .ToList();

            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            foreach (var landlord in landlords)
            {
                landlord.FloorList = new SelectList(db.tb_floor.Where(r => r.fl_active == "active"), "fl_id", "fl_bname");
            }
            return View(landlords);
        }

        


        
        [HttpPost]
        public ActionResult Pay(int id, double amount, string method, int floor)
        {

            // Retrieve the landlord from the database
            var landlord = db.tb_landlord.Find(id);

            if (landlord == null)
            {
                // Landlord not found, handle the error accordingly
                return HttpNotFound();
            }

            var userId = Convert.ToInt32(Session["id"]);

            var financeTransaction = new tb_finance
            {
                f_floor = floor, // Modify as per your requirement
                f_date = DateTime.Now, // Set the finance transaction date to the current date
                f_transaction = amount, // Set the transaction amount as per your requirement
                f_transactiontype = "Outflow", // Set the transaction type as per your requirement
                f_paymentmethod = method,
                f_user = userId,
                f_desc = "sewa Owner - " + landlord.l_name
            };

            db.tb_finance.Add(financeTransaction);
            db.SaveChanges();

            // Set a success message to be displayed on the index page
            TempData["success"] = "Payment processed successfully!";

            // Redirect back to the index page
            return RedirectToAction("Index");
        }


        // GET: Landlord/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var landlord = db.tb_landlord
                .Where(l => l.l_id == id && l.l_active != "0")
                .Include(l => l.tb_bankname)
                .FirstOrDefault();
            if (landlord == null)
            {
                return HttpNotFound();
            }
            return View(landlord);
        }

        // GET: Landlord/Create
        public ActionResult Create()
        {
            ViewBag.l_bankname = new SelectList(db.tb_bankname, "bn_id", "bn_description");
            return View();
        }

        // POST: Landlord/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "l_id,l_name,l_phone,l_fee,l_due,l_bank,l_active,l_bankname")] tb_landlord tb_landlord)
        {
            if (ModelState.IsValid)
            {
                db.tb_landlord.Add(tb_landlord);
                db.SaveChanges();
                TempData["success"] = "Landlord is successfully saved!";
                return RedirectToAction("Index");
            }

            return View(tb_landlord);
        }

        // GET: Landlord/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            if (tb_landlord == null)
            {
                return HttpNotFound();
            }

            ViewBag.l_bankname = new SelectList(db.tb_bankname, "bn_id", "bn_description", tb_landlord.l_bankname);
            
            ViewBag.l_active = tb_landlord.l_active;
            return View(tb_landlord);
        }

        // POST: Landlord/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "l_id,l_name,l_phone,l_fee,l_due,l_bank,l_active,l_bankname")] tb_landlord tb_landlord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_landlord).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Landlord is successfully saved!";
                return RedirectToAction("Index");
            }
            ViewBag.l_bankname = new SelectList(db.tb_bankname, "bn_id", "bn_description", tb_landlord.l_bankname);
            ViewBag.l_active = tb_landlord.l_active;
            return View(tb_landlord);
        }

        // GET: Landlord/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var landlord = db.tb_landlord
                .Where(l => l.l_id == id && l.l_active != "0")
                .Include(l => l.tb_bankname)
                .FirstOrDefault();
            if (landlord == null)
            {
                return HttpNotFound();
            }
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(landlord);
        }
        
        // POST: Landlord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            //db.tb_landlord.Remove(tb_landlord);
            tb_landlord.l_active = "0"; // Update l_active to "0"
            db.Entry(tb_landlord).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Landlord is deleted.";
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
