using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class FloorController : Controller
    {
        private Entities db = new Entities();

        // GET: Floor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FloorList()
        {
            var tb_floor = db.tb_floor.Include(t => t.tb_landlord);
            return View(tb_floor.ToList());
        }

            // GET: Floor/Details/5
            public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_floor tb_floor = db.tb_floor.Find(id);
            if (tb_floor == null)
            {
                return HttpNotFound();
            }
            return View(tb_floor);
        }

        public ActionResult Details_DL1()
        {
            using (var context = new Entities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals("DL1-v1.5")).ToList();
                return View(floor);
            }
        }

        // GET: Floor/Create
        public ActionResult Create()
        {
            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name");
            return View();
        }

        // POST: Floor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_layout")] tb_floor tb_floor)
        {
            if (ModelState.IsValid)
            {
                db.tb_floor.Add(tb_floor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
            return View(tb_floor);
        }

        // GET: Floor/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_floor tb_floor = db.tb_floor.Find(id);
            if (tb_floor == null)
            {
                return HttpNotFound();
            }
            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
            return View(tb_floor);
        }

        // POST: Floor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_layout")] tb_floor tb_floor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_floor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
            return View(tb_floor);
        }

        // GET: Floor/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_floor tb_floor = db.tb_floor.Find(id);
            if (tb_floor == null)
            {
                return HttpNotFound();
            }
            return View(tb_floor);
        }

        // POST: Floor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tb_floor tb_floor = db.tb_floor.Find(id);
            db.tb_floor.Remove(tb_floor);
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
