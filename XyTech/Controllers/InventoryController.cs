using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class InventoryController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Inventory
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_outdate <= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            var tb_inventory = db.tb_inventory.Where(t => t.iv_active == "1").Include(t => t.tb_floor);
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_inventory.ToList());
        }

        // GET: Inventory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            if (tb_inventory == null)
            {
                return HttpNotFound();
            }
            return View(tb_inventory);
        }

        // GET: Inventory/Create
        public ActionResult Create()
        {
            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname");
            return View();
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "iv_id,iv_floor,iv_item,iv_count,iv_active")] tb_inventory tb_inventory)
        {
            if (ModelState.IsValid)
            {
                db.tb_inventory.Add(tb_inventory);
                db.SaveChanges();
                TempData["success"] = "Inventory is successfully saved!";
                return RedirectToAction("Index");
            }

            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_inventory.iv_floor);
            return View(tb_inventory);
        }

        // GET: Inventory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            if (tb_inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_inventory.iv_floor);
            return View(tb_inventory);
        }

        // POST: Inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "iv_id,iv_floor,iv_item,iv_count,iv_active")] tb_inventory tb_inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_inventory).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Inventory is successfully saved!";
                return RedirectToAction("Index");
            }
            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_inventory.iv_floor);
            return View(tb_inventory);
        }

        // GET: Inventory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            if (tb_inventory == null)
            {
                return HttpNotFound();
            }
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            //db.tb_inventory.Remove(tb_inventory);
            tb_inventory.iv_active = "0"; // Update l_active to "0"
            db.Entry(tb_inventory).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Inventory is deleted.";
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
