using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class InvestorController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Investor
        public ActionResult Index()
        {
            var tb_investor = db.tb_investor.Include(t => t.tb_user);
            return View(tb_investor.ToList());
        }

        // GET: Investor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investor tb_investor = db.tb_investor.Find(id);
            if (tb_investor == null)
            {
                return HttpNotFound();
            }
            return View(tb_investor);
        }

        // GET: Investor/Create
        public ActionResult Create()
        {
            ViewBag.i_user = new SelectList(db.tb_user, "u_id", "u_username");
            return View();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "i_id,i_user,i_lot,i_amount,i_active")] tb_investor tb_investor)
        {
            if (ModelState.IsValid)
            {
                db.tb_investor.Add(tb_investor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.i_user = new SelectList(db.tb_user, "u_id", "u_username", tb_investor.i_user);
            return View(tb_investor);
        }

        // GET: Investor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investor tb_investor = db.tb_investor.Find(id);
            if (tb_investor == null)
            {
                return HttpNotFound();
            }
            ViewBag.i_user = new SelectList(db.tb_user, "u_id", "u_username", tb_investor.i_user);
            return View(tb_investor);
        }

        // POST: Investor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "i_id,i_user,i_lot,i_amount,i_active")] tb_investor tb_investor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_investor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.i_user = new SelectList(db.tb_user, "u_id", "u_username", tb_investor.i_user);
            return View(tb_investor);
        }

        // GET: Investor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investor tb_investor = db.tb_investor.Find(id);
            if (tb_investor == null)
            {
                return HttpNotFound();
            }
            return View(tb_investor);
        }

        // POST: Investor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_investor tb_investor = db.tb_investor.Find(id);
            db.tb_investor.Remove(tb_investor);
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
