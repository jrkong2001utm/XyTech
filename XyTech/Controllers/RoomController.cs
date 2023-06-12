using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class RoomController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Room
        public ActionResult Index()
        {
            var tb_room = db.tb_room.Include(t => t.tb_floor);
            return View(tb_room.ToList());
        }

        // GET: Room/Details/5
        public ActionResult Details(int? id)
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

        // GET: Room/Create
        public ActionResult Create()
        {
            ViewBag.r_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname");
            return View();
        }

        // POST: Room/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "r_id,r_floor,r_price,r_availability,r_size,r_active,r_pic,r_no")] tb_room tb_room, HttpPostedFileBase roompic)
        {
            if (ModelState.IsValid)
            {
                if (roompic != null && roompic.ContentLength > 0)
                {
                    if (roompic.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(roompic.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/roomPicture"), _FileName);
                        roompic.SaveAs(_path);
                        tb_room.r_pic = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_room);
                    }
                }

                db.tb_room.Add(tb_room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.r_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_room.r_floor);
            return View(tb_room);
        }

        // GET: Room/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.r_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_room.r_floor);
            return View(tb_room);
        }

        // POST: Room/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "r_id,r_floor,r_price,r_availability,r_size,r_active,r_pic,r_no")] tb_room tb_room, HttpPostedFileBase roompic)
        {
            if (ModelState.IsValid)
            {
                if (roompic != null && roompic.ContentLength > 0)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/assets/images/roomPicture"), tb_room.r_pic));
                    if (roompic.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(roompic.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/roomPicture"), _FileName);
                        roompic.SaveAs(_path);
                        tb_room.r_pic = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_room);
                    }
                }

                db.Entry(tb_room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.r_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_room.r_floor);
            return View(tb_room);
        }

        // GET: Room/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_room tb_room = db.tb_room.Find(id);
            db.tb_room.Remove(tb_room);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetFile(string FileName)
        {
            string qrfilePath = Server.MapPath("~/Content/assets/images/cctvqr/" + FileName);

            if (System.IO.File.Exists(qrfilePath))
            {
                return File(qrfilePath, "image/png"); // Adjust the content type according to the actual file type
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
