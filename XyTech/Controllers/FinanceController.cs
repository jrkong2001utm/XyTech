using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class FinanceController : Controller
    {
        private Entities db = new Entities();

        // GET: Finance
        public ActionResult Index()
        {
            var tb_finance = db.tb_finance.Include(t => t.tb_user).Include(t => t.tb_floor);
            return View(tb_finance.ToList());
        }

        // GET: Finance/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_finance tb_finance = db.tb_finance.Find(id);
            if (tb_finance == null)
            {
                return HttpNotFound();
            }
            return View(tb_finance);
        }

        // GET: Finance/Create
        public ActionResult Create()
        {
            ViewBag.f_user = Session["u_username"];
            ViewBag.f_floor = new SelectList(db.tb_floor, "fl_id", "fl_id");
            return View();
        }

        // POST: Finance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "f_id,f_floor,f_user,f_transaction,f_desc,f_transactiontype,f_paymentmethod,f_date,f_receipt")] tb_finance tb_finance)
        {
            if (ModelState.IsValid)
            {
                db.tb_finance.Add(tb_finance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.f_user = new SelectList(db.tb_user, "u_username", "u_pwd", tb_finance.f_user);
            ViewBag.f_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid", tb_finance.f_floor);
            return View(tb_finance);
        }

        // GET: Finance/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_finance tb_finance = db.tb_finance.Find(id);
            if (tb_finance == null)
            {
                return HttpNotFound();
            }
            ViewBag.f_user = new SelectList(db.tb_user, "u_username", "u_pwd", tb_finance.f_user);
            ViewBag.f_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid", tb_finance.f_floor);
            return View(tb_finance);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "f_id,f_floor,f_user,f_transaction,f_desc,f_transactiontype,f_paymentmethod,f_date,f_receipt")] tb_finance tb_finance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_finance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.f_user = new SelectList(db.tb_user, "u_username", "u_pwd", tb_finance.f_user);
            ViewBag.f_floor = new SelectList(db.tb_floor, "fl_id", "fl_bid", tb_finance.f_floor);
            return View(tb_finance);
        }

        // GET: Finance/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_finance tb_finance = db.tb_finance.Find(id);
            if (tb_finance == null)
            {
                return HttpNotFound();
            }
            return View(tb_finance);
        }

        // POST: Finance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_finance tb_finance = db.tb_finance.Find(id);
            db.tb_finance.Remove(tb_finance);
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
