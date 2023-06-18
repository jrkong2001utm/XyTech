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
    public class InventoryController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Inventory
        public ActionResult Index(int? floorFilter)
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Month != DateTime.Today.Month && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }

            var floorOptions = db.tb_floor.Where(f => f.fl_active == "active").Select(f => new SelectListItem
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

            IQueryable<tb_inventory> tb_inventoryQuery = db.tb_inventory.Include(t => t.tb_floor).Where(p => p.iv_active == "1");

            if (floorFilter.HasValue)
            {
                tb_inventoryQuery = tb_inventoryQuery.Where(p => p.iv_floor == floorFilter);
            }

            var tb_inventory = tb_inventoryQuery.ToList();
            return View(tb_inventory);

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
            var inventory = new tb_inventory(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                // Assign values to other properties of the inventory object if needed

            ViewBag.iv_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", inventory.iv_floor);


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
                TempData["success"] = "Inventory is successfully saved!";
                return RedirectToAction("Index");
            }

            var inventory = new tb_inventory(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                // Assign values to other properties of the inventory object if needed

            ViewBag.iv_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", inventory.iv_floor);


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
            var inventory = new tb_inventory(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                // Assign values to other properties of the inventory object if needed
            var selectedFloorId = tb_inventory.iv_floor;
            ViewBag.iv_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", selectedFloorId);

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
                TempData["success"] = "Inventory is successfully saved!";
                return RedirectToAction("Index");
            }
            var inventory = new tb_inventory(); // Replace tb_inventory with the appropriate class name and constructor if necessary
                                                // Assign values to other properties of the inventory object if needed
            var selectedFloorId = tb_inventory.iv_floor;
            ViewBag.iv_floor = new SelectList(db.tb_floor.Where(f => f.fl_active == "active"), "fl_id", "fl_bname", selectedFloorId);

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
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_inventory tb_inventory = db.tb_inventory.Find(id);
            //db.tb_inventory.Remove(tb_inventory);
            tb_inventory.iv_active = "0"; // Update l_active to "0"
            db.Entry(tb_inventory).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Inventory is deleted.";
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
