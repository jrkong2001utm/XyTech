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
    public class LandlordController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Landlord
        public ActionResult Index()
        {
            var landlords = db.tb_landlord
                .Where(l => l.l_active != "4")
                .Include(l => l.tb_bankname)
                .OrderBy(l => l.l_due)
                .ToList();

            return View(landlords);
        }

        // GET: Landlord/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var landlord = db.tb_landlord
                .Where(l => l.l_id == id && l.l_active != "4")
                .Include(l => l.tb_bankname)
                .FirstOrDefault();
            if (landlord == null)
            {
                return HttpNotFound();
            }
            return View(landlord);
        }

        // GET: Landlord/Create
        public ActionResult Create()
        {
            ViewBag.l_bankname = new SelectList(db.tb_bankname, "bn_id", "bn_description");
            return View();
        }

        // POST: Landlord/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "l_id,l_name,l_phone,l_fee,l_due,l_bank,l_active,l_bankname")] tb_landlord tb_landlord)
        {
            if (ModelState.IsValid)
            {
                db.tb_landlord.Add(tb_landlord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_landlord);
        }

        // GET: Landlord/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            if (tb_landlord == null)
            {
                return HttpNotFound();
            }
            return View(tb_landlord);
        }

        // POST: Landlord/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "l_id,l_name,l_phone,l_fee,l_due,l_bank,l_active")] tb_landlord tb_landlord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_landlord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_landlord);
        }

        // GET: Landlord/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            if (tb_landlord == null)
            {
                return HttpNotFound();
            }
            return View(tb_landlord);
        }

        // POST: Landlord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_landlord tb_landlord = db.tb_landlord.Find(id);
            //db.tb_landlord.Remove(tb_landlord);
            tb_landlord.l_active = "4"; // Update l_active to "4"
            db.Entry(tb_landlord).State = EntityState.Modified;
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
