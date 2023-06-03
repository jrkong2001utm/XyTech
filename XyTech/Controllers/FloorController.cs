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
    public class FloorController : Controller
    {
        private Entities db = new Entities();

        // GET: Floor
        public ActionResult Index()
        {
            var tb_floor = db.tb_floor.Include(t => t.tb_landlord);
            return View(tb_floor.ToList());
        }

        // GET: Floor/Details/5
        public ActionResult Details_DL1(string searchString)
        {
            var floor = from f in db.tb_floor.Include(t => t.tb_landlord)
                        select f;

            if (!String.IsNullOrEmpty(searchString))
            {
                floor = floor.Where(f => f.fl_id.Contains(searchString));
            }
            return View(floor.ToList());
        }

        public ActionResult Room(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_room tb_room = db.tb_room.Find(id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            return View(tb_room);
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
        public ActionResult Create([Bind(Include = "fl_id,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_bid,fl_layout")] tb_floor tb_floor)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fl_id,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_bid,fl_layout")] tb_floor tb_floor)
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
