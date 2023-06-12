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
    public class AttendanceController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Attendance
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_outdate <= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            var tb_attendance = db.tb_attendance.Include(t => t.tb_floor);
            return View(tb_attendance.ToList());
        }

        public ActionResult GetFilteredData(DateTime? startDate, DateTime? endDate)
        {
            var query = db.tb_attendance.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(a => a.a_check >= startDate && a.a_check <= endDate);
            }

            var data = query.Select(a => new
            {
                a_check = a.a_check,
                a_floor = a.a_floor,
                fl_bname = a.tb_floor.fl_bname,
                a_id = a.a_id
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Other controller methods...

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Attendance/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            if (tb_attendance == null)
            {
                return HttpNotFound();
            }
            return View(tb_attendance);
        }

        // GET: Attendance/Create
        public ActionResult Create()
        {
            ViewBag.a_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname");
            return View();
        }

        // POST: Attendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "a_id,a_floor,a_check")] tb_attendance tb_attendance)
        {
            if (ModelState.IsValid)
            {
                db.tb_attendance.Add(tb_attendance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.a_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_attendance.a_floor);
            return View(tb_attendance);
        }

        // GET: Attendance/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            if (tb_attendance == null)
            {
                return HttpNotFound();
            }
            ViewBag.a_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_attendance.a_floor);
            return View(tb_attendance);
        }

        // POST: Attendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "a_id,a_floor,a_check")] tb_attendance tb_attendance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_attendance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.a_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_attendance.a_floor);
            return View(tb_attendance);
        }

        // GET: Attendance/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            if (tb_attendance == null)
            {
                return HttpNotFound();
            }
            return View(tb_attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_attendance tb_attendance = db.tb_attendance.Find(id);
            db.tb_attendance.Remove(tb_attendance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
