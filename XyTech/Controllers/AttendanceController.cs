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
        public ActionResult Index(int? floorFilter)
        {
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");

            var floorOptions = db.tb_floor
                .Where(f => f.fl_active.Equals("active"))
                .Select(f => new SelectListItem
                {
                    Text = f.fl_bname,
                    Value = f.fl_id.ToString(),
                    Selected = (floorFilter.HasValue && floorFilter.Value == f.fl_id)
                })
                .ToList();

            ViewBag.FloorFilter = floorOptions;

            var tb_attendance = db.tb_attendance.Include(t => t.tb_floor);
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_attendance.ToList());
        }

        public ActionResult GetFilteredData(DateTime? startDate, DateTime? endDate, int? floorFilter)
        {
            var query = db.tb_attendance.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(a => a.a_check >= startDate && a.a_check <= endDate);
            }

            if (floorFilter.HasValue && floorFilter.Value != 0)
            {
                query = query.Where(a => a.a_floor == floorFilter.Value);
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
            var attendance = new tb_attendance(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                // Assign values to other properties of the inventory object if needed

            ViewBag.a_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", attendance.a_floor);
            
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
                DateTime checkDate = tb_attendance.a_check.Date;
                int month = checkDate.Month;
                int year = checkDate.Year;
                int fid = tb_attendance.a_floor;

                int count = db.tb_attendance
                    .Count(a => a.a_check.Month == month && a.a_check.Year == year && a.a_floor == fid);

                if (count >= 3)
                {
                    TempData["ErrorMessage"] = "The attendance entries of the month reaches or exceeds 3 times";
                    db.tb_attendance.Add(tb_attendance);
                    TempData["success"] = "Attendance is successfully saved!";
                    db.SaveChanges();
                    
                }
                else
                {
                    db.tb_attendance.Add(tb_attendance);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
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
            var attendance = new tb_attendance(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                  // Assign values to other properties of the inventory object if needed
            var selectedFloorId = tb_attendance.a_floor;
            ViewBag.a_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", selectedFloorId);
            
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
                TempData["success"] = "Attendance is successfully saved!";
                return RedirectToAction("Index");
            }
            var attendance = new tb_attendance(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                  // Assign values to other properties of the inventory object if needed
            var selectedFloorId = tb_attendance.a_floor;
            ViewBag.a_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", selectedFloorId);
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
            TempData["success"] = "Attendance is deleted.";
            return RedirectToAction("Index");
        }
    }
}
