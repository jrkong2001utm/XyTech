using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;
using System.Globalization;
using XyTech.Attributes;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class ProfitController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Profit
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_indate.Day >= DateTime.Today.Day && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            if (Session["usertype"] != null && Session["usertype"].Equals("Investor"))
            {
                var userId = Convert.ToInt32(Session["id"]);
                var tb_investor = db.tb_investor.FirstOrDefault(i => i.i_user == userId);

                if (tb_investor != null)
                {
                    int i_id = tb_investor.i_id;
                    var currentMonth = DateTime.Now.Month;
                    var currentYear = DateTime.Now.Year;
                    var month = $"{currentYear}-{currentMonth}";

                    var tb_profit = db.tb_profit
                        .Where(p => p.p_investor == i_id && !p.p_month.Equals(month))
                        .ToList();

                    if (tb_profit != null)
                    {
                        foreach (var profitEntry in tb_profit)
                        {
                            // Retrieve investor name from tb_user
                            var investor = db.tb_user.FirstOrDefault(u => u.u_id == tb_investor.i_user);
                            if (investor != null)
                                profitEntry.InvestorUsername = investor.u_username;
                        }

                        tb_profit = tb_profit
                            .OrderByDescending(p => Convert.ToInt32(p.p_month.Split('-')[0])) // Order by year
                            .ThenByDescending(p => Convert.ToInt32(p.p_month.Split('-')[1])) // Order by month
                            .ToList();

                        return View(tb_profit);
                    }
                    else
                    {
                        return View();
                    }
                }
            }

            var profit = db.tb_profit
                .Include(t => t.tb_investor)
                .ToList()
                .Select(p => new tb_profit
                {
                    p_id = p.p_id,
                    p_investor = p.p_investor,
                    p_month = p.p_month,
                    p_profit = p.p_profit,
                    p_lot = p.tb_investor.i_lot,
                    InvestorUsername = db.tb_user.FirstOrDefault(u => u.u_id == p.tb_investor.i_user)?.u_username
                })
                .OrderByDescending(p => Convert.ToInt32(p.p_month.Split('-')[0])) // Order by year
                .ThenByDescending(p => Convert.ToInt32(p.p_month.Split('-')[1])) // Order by month
                .ToList();

            return View(profit);
            
        }


        // GET: Profit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_profit tb_profit = db.tb_profit.Find(id);
            if (tb_profit == null)
            {
                return HttpNotFound();
            }
            return View(tb_profit);
        }

        // GET: Profit/Create
        public ActionResult Create()
        {
            ViewBag.p_investor = new SelectList(db.tb_investor, "i_id", "i_active");
            return View();
        }

        // POST: Profit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "p_id,p_investor,p_month,p_profit")] tb_profit tb_profit)
        {
            if (ModelState.IsValid)
            {
                db.tb_profit.Add(tb_profit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.p_investor = new SelectList(db.tb_investor, "i_id", "i_active", tb_profit.p_investor);
            return View(tb_profit);
        }

        // GET: Profit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_profit tb_profit = db.tb_profit.Find(id);
            if (tb_profit == null)
            {
                return HttpNotFound();
            }
            ViewBag.p_investor = new SelectList(db.tb_investor, "i_id", "i_active", tb_profit.p_investor);
            return View(tb_profit);
        }

        // POST: Profit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "p_id,p_investor,p_month,p_profit")] tb_profit tb_profit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_profit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.p_investor = new SelectList(db.tb_investor, "i_id", "i_active", tb_profit.p_investor);
            return View(tb_profit);
        }

        // GET: Profit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_profit tb_profit = db.tb_profit.Find(id);
            if (tb_profit == null)
            {
                return HttpNotFound();
            }
            return View(tb_profit);
        }

        // POST: Profit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_profit tb_profit = db.tb_profit.Find(id);
            db.tb_profit.Remove(tb_profit);
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
