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
