using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Attributes;
using XyTech.Models;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class RoomController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Room
        public ActionResult Index(int? floorFilter)
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }

            var floorOptions = db.tb_floor.Select(f => new SelectListItem
            {
                Text = f.fl_bname,
                Value = f.fl_id.ToString(),
                Selected = (floorFilter.HasValue && floorFilter.Value == f.fl_id)
            }).ToList();

            // Add a default option at the beginning of the list
            floorOptions.Insert(0, new SelectListItem
            {
                Text = "All Floors",
                Value = null,
                Selected = (!floorFilter.HasValue)
            });

            ViewData["FloorFilter"] = floorOptions;

            IQueryable<tb_room> tb_roomQuery = db.tb_room.Include(t => t.tb_floor).Where(p => p.r_active.Equals("active"));

            if (floorFilter.HasValue)
            {
                tb_roomQuery = tb_roomQuery.Where(p => p.r_floor == floorFilter);
            }

            var tb_roomList = tb_roomQuery.ToList();
            return View(tb_roomList);
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
            var room = new tb_room(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                        // Assign values to other properties of the inventory object if needed

            ViewBag.r_floor = new SelectList(db.tb_floor.Where(r => r.fl_active == "active"), "fl_id", "fl_bname", room.r_floor);

            //ViewBag.r_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname");
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
                TempData["success"] = " Room has been created successfully!";
                return RedirectToAction("Index");
            }

            var room = new tb_room(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                      // Assign values to other properties of the inventory object if needed

            ViewBag.r_floor = new SelectList(db.tb_floor.Where(r => r.fl_active == "active"), "fl_id", "fl_bname", room.r_floor);

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
            var room = new tb_room(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                      // Assign values to other properties of the inventory object if needed
            var selectedFloorId = tb_room.r_floor;
            ViewBag.r_floor = new SelectList(db.tb_floor.Where(r => r.fl_active == "active"), "fl_id", "fl_bname", selectedFloorId);

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
                TempData["success"] = " Room has been updated successfully!";
                return RedirectToAction("Index");
            }
            var room = new tb_room(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                      // Assign values to other properties of the inventory object if needed

            var selectedFloorId = tb_room.r_floor;
            ViewBag.r_floor = new SelectList(db.tb_floor.Where(r => r.fl_active == "active"), "fl_id", "fl_bname", selectedFloorId);

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
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_room tb_room = db.tb_room.Find(id);
            tb_room.r_active = "inactive"; 
            db.Entry(tb_room).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = " Room has been deleted successfully!";
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
