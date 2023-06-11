using System;
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
using XyTech.Models;
using static System.Net.WebRequestMethods;

namespace XyTech.Controllers
{

    public class FloorController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

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
            return View(tb_floor);
        }

        public ActionResult Details_DL1()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(1)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_DL5()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(2)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_GE1()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(3)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_GE2()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(4)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_GE3()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(5)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_TN1()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(6)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_TN2()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(7)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_TN3()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(8)).ToList();
                return View(floor);
            }
        }

        public ActionResult Details_TN4()
        {
            using (var context = new db_XyTechEntities())
            {
                var floor = context.tb_floor.Include(t => t.tb_landlord).Where(p => p.fl_id.Equals(9)).ToList();
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
        public ActionResult Create([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_layout")] tb_floor tb_floor, HttpPostedFileBase uploadqr, HttpPostedFileBase uploadly)
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
                TempData["success"] = "Tenant saved successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
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
            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
            return View(tb_floor);
        }

        // POST: Floor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_layout")] tb_floor tb_floor, HttpPostedFileBase uploadqr, HttpPostedFileBase uploadly)
        {

            if (ModelState.IsValid)
            {
                if (uploadqr != null && uploadqr.ContentLength > 0)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/assets/images/cctvqr"), tb_floor.fl_cctvqr));
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
                    if (uploadqr.ContentType.Contains("image"))
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
                TempData["success"] = "Floor updated successfully!";
                return RedirectToAction("FloorList");
            }

            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
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
            return View(tb_floor);
        }

        // POST: Floor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var context = new db_XyTechEntities())
    {
        // Find the floor record with the specified id
        var floor = context.tb_floor.Find(id);

        if (floor != null)
        {
            // Set the fl_active attribute to "inactive"
            floor.fl_active = "inactive";

            // Save the changes to the database
            context.SaveChanges();
        }
    }

            return RedirectToAction("Index");
        }

        public ActionResult GetFile(string FileName)
        {
            string qrfilePath = Server.MapPath("~/Content/assets/images/cctvqr/" + FileName);
            string layoutfilePath = Server.MapPath("~/Content/assets/images/floorlayout/" + FileName);

            if (System.IO.File.Exists(qrfilePath))
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
