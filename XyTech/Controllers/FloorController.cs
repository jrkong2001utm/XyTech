﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using XyTech.Attributes;
using XyTech.Models;
using static System.Net.WebRequestMethods;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class FloorController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Floor
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Month != DateTime.Today.Month && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            return View();
        }

        public ActionResult FloorList()
        {
            var tb_floor = db.tb_floor.Include(t => t.tb_landlord).Where(t => t.fl_active.Equals("active"));
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_floor.ToList());
        }

        // GET: Floor/Details/5
        public ActionResult Details(int? id)
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
            ViewBag.room = db.tb_room.Where(r => r.r_active == "active" && r.r_floor == tb_floor.fl_id).ToList();
            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
            return View(tb_floor);
        }

        public ActionResult Details_DL1()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 1).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(1)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_DL5()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 2).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(2)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_GE1()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 3).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(3)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_GE2()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 4).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(4)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_GE3()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 5).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(5)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_TN1()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 6).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(6)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_TN2()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 7).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(7)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_TN3()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 8).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(8)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        public ActionResult Details_TN4()
        {
            using (var context = new db_XyTechEntities())
            {
                ViewBag.room = context.tb_room.Where(r => r.r_active == "active" && r.r_floor == 9).ToList();
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(9)).FirstOrDefault();
                ViewBag.fl_landlord = new SelectList(context.tb_landlord, "l_id", "l_name", floor.fl_landlord);
                return View(floor);
            }
        }

        // GET: Floor/Create
        public ActionResult Create()
        {
            var floor = new tb_floor(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                        // Assign values to other properties of the inventory object if needed

            ViewBag.fl_landlord = new SelectList(db.tb_landlord.Where(l => l.l_active == "1"), "l_id", "l_name", floor.fl_landlord);

            //ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name");
            return View();
        }

        // POST: Floor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_bname,fl_address,fl_active,fl_layout,fl_preset_potential,fl_preset_current")] tb_floor tb_floor, HttpPostedFileBase uploadqr, HttpPostedFileBase uploadly)
        {

            //HttpPostedFileBase imageFile = Request.Files["imageFile"];
            //ttpPostedFileBase qrimageFile = Request.Files["qrimageFile"];

            if (ModelState.IsValid)
            {
                if (uploadqr != null && uploadqr.ContentLength > 0)
                {
                    if (uploadqr.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(uploadqr.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/cctvqr"), _FileName);
                        uploadqr.SaveAs(_path);
                        tb_floor.fl_cctvqr = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_floor);
                    }
                }

                if (uploadly != null && uploadly.ContentLength > 0)
                {
                    if (uploadly.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(uploadly.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/floorlayout"), _FileName);
                        uploadly.SaveAs(_path);
                        tb_floor.fl_layout = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_floor);
                    }
                }

                db.tb_floor.Add(tb_floor);
                db.SaveChanges();
                TempData["success"] = " Floor has been created successfully!";
                return RedirectToAction("FloorList");
            }

            var floor = new tb_floor(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                        // Assign values to other properties of the inventory object if needed

            ViewBag.fl_landlord = new SelectList(db.tb_landlord.Where(l => l.l_active == "1"), "l_id", "l_name", floor.fl_landlord);

            return View(tb_floor);
        }

        // GET: Floor/Edit/5
        public ActionResult Edit(int? id)
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
            var floor = new tb_floor(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                        // Assign values to other properties of the inventory object if needed
            var selectedLId = tb_floor.fl_landlord;
            ViewBag.fl_landlord = new SelectList(db.tb_landlord.Where(l => l.l_active == "1"), "l_id", "l_name", selectedLId);
            return View(tb_floor);
        }

        // POST: Floor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_bname,fl_address,fl_active,fl_layout,fl_preset_potential,fl_preset_current")] tb_floor tb_floor, HttpPostedFileBase uploadqr, HttpPostedFileBase uploadly)
        {

            if (ModelState.IsValid)
            {
                /*if (uploadqr != null && uploadqr.ContentLength > 0)
                {
                    if (uploadqr.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(uploadqr.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/cctvqr"), _FileName);
                        uploadqr.SaveAs(_path);
                        tb_floor.fl_cctvqr = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_floor);
                    }
                }*/

                if (uploadly != null && uploadly.ContentLength > 0)
                {
                    if (uploadly.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(uploadly.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/floorlayout"), _FileName);
                        uploadly.SaveAs(_path);
                        tb_floor.fl_layout = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_floor);
                    }
                }

                db.Entry(tb_floor).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = " Floor has been updated successfully!";
                return RedirectToAction("FloorList");
            }

            var floor = new tb_floor(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                        // Assign values to other properties of the inventory object if needed

            var selectedLId = tb_floor.fl_landlord;
            ViewBag.fl_landlord = new SelectList(db.tb_landlord.Where(l => l.l_active == "1"), "l_id", "l_name", selectedLId);

            return View(tb_floor);
        }


        // GET: Floor/Delete/5
        public ActionResult Delete(int? id)
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
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_floor);
        }

        // POST: Floor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_floor tb_floor = db.tb_floor.Find(id);
            tb_floor.fl_active = "inactive";
            db.Entry(tb_floor).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = " Floor has been deleted successfully!";
            return RedirectToAction("FloorList");
        }

        public ActionResult GetFile(string FileName)
        {
            string qrfilePath = Server.MapPath("~/Content/assets/images/cctvqr/" + FileName);
            string layoutfilePath = Server.MapPath("~/Content/assets/images/floorlayout/" + FileName);

            if (System.IO.File.Exists(layoutfilePath))
            {
                return File(layoutfilePath, "image/png"); // Adjust the content type according to the actual file type
            }
            else
            {
                return HttpNotFound();
            }
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