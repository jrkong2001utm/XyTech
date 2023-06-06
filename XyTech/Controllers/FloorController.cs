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
            tb_floor floor = new tb_floor();

            HttpPostedFileBase imageFile = Request.Files["imageFile"];
            HttpPostedFileBase qrimageFile = Request.Files["qrimageFile"];

            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Read the image file into a byte array
                    byte[] imageData;
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(imageFile.ContentLength);
                    }

                    // Assign the image data to the floor object
                    floor.fl_layout = imageData;
                }

                if (qrimageFile != null && qrimageFile.ContentLength > 0)
                {
                    // Read the image file into a byte array
                    byte[] qrimageData;
                    using (var binaryReader = new BinaryReader(qrimageFile.InputStream))
                    {
                        qrimageData = binaryReader.ReadBytes(qrimageFile.ContentLength);
                    }

                    // Assign the image data to the floor object
                    floor.fl_cctvqr = qrimageData;
                }

                using (var context = new db_XyTechEntities())
                {
                    context.tb_floor.Add(tb_floor);
                    context.SaveChanges();
                }

                return RedirectToAction("FloorList");
            }
               
            ViewBag.fl_landlord = new SelectList(db.tb_landlord, "l_id", "l_name", tb_floor.fl_landlord);
            return View(floor);
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
        public ActionResult Edit([Bind(Include = "fl_id,fl_bid,fl_wifipwd,fl_modemip,fl_cctvqr,fl_landlord,fl_address,fl_active,fl_layout")] tb_floor tb_floor)
        {
            HttpPostedFileBase imageFile = Request.Files["imageFile"];
            HttpPostedFileBase qrimageFile = Request.Files["qrimageFile"];

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Read the image file into a byte array
                    byte[] imageData;
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(imageFile.ContentLength);
                    }

                    // Assign the image data to the floor object
                    tb_floor.fl_layout = imageData;
                }

                if (qrimageFile != null && qrimageFile.ContentLength > 0)
                {
                    // Read the image file into a byte array
                    byte[] qrimageData;
                    using (var binaryReader = new BinaryReader(qrimageFile.InputStream))
                    {
                        qrimageData = binaryReader.ReadBytes(qrimageFile.ContentLength);
                    }

                    // Assign the image data to the floor object
                    tb_floor.fl_cctvqr = qrimageData;
                }

                db.Entry(tb_floor).State = EntityState.Modified;
                db.SaveChanges();

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
