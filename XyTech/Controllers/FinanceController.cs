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
    //[CustomAuthorize]
    public class FinanceController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Finance
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_outdate <= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            var tb_finance = db.tb_finance
                .Include(t => t.tb_user)
                .Include(t => t.tb_floor)
                .OrderByDescending(t => t.f_date); // Order by f_date in descending order
            return View(tb_finance.ToList());
        }

        public ActionResult Summary()
        {
            var financeData = db.tb_finance
                .GroupBy(t => new { t.f_date.Year, t.f_date.Month, t.f_floor })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    FloorId = g.Key.f_floor,
                    Profit = g.Sum(t => t.f_transaction * (t.f_transactiontype == "Inflow" ? 1 : -1))
                })
                .OrderBy(t => t.FloorId)
                .ToList();

            var profitByFloor = new Dictionary<int, Dictionary<int, Dictionary<string, double>>>();

            var floorNames = db.tb_floor.ToDictionary(f => f.fl_id, f => f.fl_bname);

            foreach (var financeEntry in financeData)
            {
                var year = financeEntry.Year;
                var month = financeEntry.Month;
                var floorId = financeEntry.FloorId.HasValue ? financeEntry.FloorId.Value : 0;
                var floorName = floorNames.TryGetValue(floorId, out var name) ? name : "General";
                var profit = financeEntry.Profit;

                if (!profitByFloor.ContainsKey(year))
                {
                    profitByFloor[year] = new Dictionary<int, Dictionary<string, double>>();
                }

                if (!profitByFloor[year].ContainsKey(month))
                {
                    profitByFloor[year][month] = new Dictionary<string, double>();
                }

                profitByFloor[year][month][floorName] = profit;
            }

            return View(profitByFloor);
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
            ViewBag.f_user = Session["id"];

            var floorList = db.tb_floor.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "select", Text = "-- Select Floor --", Disabled = true },
                new SelectListItem { Value = "0", Text = "General" }
            };
            selectList.AddRange(floorList.Select(f => new SelectListItem { Value = f.fl_id.ToString(), Text = f.fl_bname }));
            ViewBag.f_floor = new SelectList(selectList, "Value", "Text");

            return View();
        }

        // POST: Finance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "f_id,f_floor,f_user,f_transaction,f_desc,f_transactiontype,f_paymentmethod,f_date,f_receipt")] tb_finance tb_finance)
        {
            if(tb_finance.f_floor == 0)
            {
                tb_finance.f_floor = null;
            }
            if (ModelState.IsValid)
            {
                db.tb_finance.Add(tb_finance);
                db.SaveChanges();
                CalculateCurrentMonthProfit();
                return RedirectToAction("Index");
            }

            ViewBag.f_user = Session["id"];

            var floorList = db.tb_floor.ToList();
            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "select", Text = "-- Select Floor --", Disabled = true },
                new SelectListItem { Value = "0", Text = "General" }
            };
            selectList.AddRange(floorList.Select(f => new SelectListItem { Value = f.fl_id.ToString(), Text = f.fl_bname }));
            ViewBag.f_floor = new SelectList(selectList, "Value", "Text");
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
            ViewBag.f_user = Session["id"];
            ViewBag.f_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_finance.f_floor);
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
                CalculateCurrentMonthProfit();
                return RedirectToAction("Index");
            }
            ViewBag.f_user = Session["id"];
            ViewBag.f_floor = new SelectList(db.tb_floor, "fl_id", "fl_bname", tb_finance.f_floor);
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
            CalculateCurrentMonthProfit();
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

        public void CalculateCurrentMonthProfit()
        {
            //var currentMonth = DateTime.Now.Month;
            //var currentYear = DateTime.Now.Year;

            var currentDate = DateTime.Now.AddMonths(-1);
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            // Retrieve the investors from the tb_investor table
            var investors = db.tb_investor.ToList().Where(t => t.i_active == "active");

            var financeData = db.tb_finance
                .Where(t => t.f_date.Month == currentMonth && t.f_date.Year == currentYear)
                .GroupBy(t => new { t.f_floor })
                .Select(g => new
                {
                    FloorId = g.Key.f_floor,
                    Profit = g.Sum(t => t.f_transaction * (t.f_transactiontype == "Inflow" ? 1 : -1))
                })
                .ToList();

            // Calculate the total profit for the current month
            var profit = financeData.Sum(t => t.Profit);
            var partner = 3;
            var lot = db.tb_investor.Select(x => x.i_lot).Distinct().Count();
            var priceperlot = 50000;
            profit = profit * 0.6 / (partner + lot);
            var p_month = $"{currentYear}-{currentMonth}";

            foreach (var investor in investors)
            {
                var sharing = profit * (investor.i_amount / priceperlot);

                // Check if a profit entry already exists for the current month and investor
                var existingProfit = db.tb_profit.FirstOrDefault(p => p.p_investor == investor.i_id && p.p_month == p_month);

                if (existingProfit != null)
                {
                    // Update the existing profit entry
                    existingProfit.p_profit = sharing;
                    db.Entry(existingProfit).State = EntityState.Modified;
                }
                else
                {
                    // Create a new profit entry
                    var profitEntry = new tb_profit
                    {
                        p_investor = investor.i_id,
                        p_month = $"{currentYear}-{currentMonth}",
                        p_profit = sharing
                    };

                    // Add the profit entry to the table
                    db.tb_profit.Add(profitEntry);
                }
            }

            // Save the changes to the database
            db.SaveChanges();
        }
    }
}
