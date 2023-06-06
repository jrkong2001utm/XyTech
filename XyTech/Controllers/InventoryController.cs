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
            var tb_inventory = db.tb_inventory.Include(t => t.tb_floor);
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
            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid");
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
                return RedirectToAction("Index");
            }

            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid", tb_inventory.iv_floor);
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
            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid", tb_inventory.iv_floor);
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
                return RedirectToAction("Index");
            }
            ViewBag.iv_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid", tb_inventory.iv_floor);
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
            return View(tb_inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            db.tb_inventory.Remove(tb_inventory);
            db.SaveChanges();
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
