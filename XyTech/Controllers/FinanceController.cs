using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using XyTech.Attributes;
using XyTech.Models;
using static System.Net.WebRequestMethods;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class FinanceController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Finance
        public ActionResult Index(int? floorFilter)
        {
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");

            // Populate floor options for the dropdown list
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

            floorOptions.Insert(1, new SelectListItem
            {
                Text = "General",
                Value = "0",
                Selected = (floorFilter.HasValue && floorFilter.Value == 0)
            });

            // Add the floor options to ViewData
            ViewData["FloorFilter"] = floorOptions;

            if (TempData.ContainsKey("success"))
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View();
        }

        public ActionResult GetFilteredData(DateTime? startDate, DateTime? endDate, int? floorFilter)
        {
            var query = db.tb_finance.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(a => a.f_date >= startDate && a.f_date <= endDate);
            }

            if (floorFilter.HasValue && floorFilter.Value != 0)
            {
                query = query.Where(a => a.tb_floor.fl_id == floorFilter.Value);
            }
            else if (floorFilter.HasValue && floorFilter.Value == 0)
            {
                query = query.Where(a => a.f_floor == null);
            }

            var data = query.Select(a => new
            {
                f_date = a.f_date,
                f_year = a.f_date.Year,
                f_month = a.f_date.Month,
                f_day = a.f_date.Day,
                f_type = a.f_transactiontype,
                f_transaction = a.f_transaction,
                f_paymentmethod = a.f_paymentmethod,
                f_desc = a.f_desc,
                f_floor = a.tb_floor.fl_bname,
                f_id = a.f_id
            }).OrderByDescending(a => a.f_date).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
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
        public ActionResult Create([Bind(Include = "f_id,f_floor,f_user,f_transaction,f_desc,f_transactiontype,f_paymentmethod,f_date,f_receipt")] tb_finance tb_finance, HttpPostedFileBase contractfile)
        {
            if(tb_finance.f_floor == 0)
            {
                tb_finance.f_floor = null;
            }
            if (ModelState.IsValid)
            {
                if (contractfile != null && contractfile.ContentLength > 0)
                {
                    if (contractfile.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(contractfile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/Contractfile"), _FileName);
                        contractfile.SaveAs(_path);
                        tb_finance.f_receipt = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_finance);
                    }
                }

                db.tb_finance.Add(tb_finance);
                db.SaveChanges();
                TempData["success"] = "Transaction is successfully saved!";
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
        public ActionResult Edit(tb_finance updatef, HttpPostedFileBase contractfile)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(tb_finance).State = EntityState.Modified;
            //    db.SaveChanges();
            //    CalculateCurrentMonthProfit();
            //    return RedirectToAction("Index");
            //}
            
            //return View(tb_finance);

            tb_finance tb_finance = db.tb_finance.Find(updatef.f_id);
            if (tb_finance == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {

                if (contractfile != null && contractfile.ContentLength > 0)
                {
                    if (contractfile.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(contractfile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/Contractfile"), _FileName);
                        contractfile.SaveAs(_path);
                        tb_finance.f_receipt = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_finance);
                    }
                }
                tb_finance.f_id = updatef.f_id;
                tb_finance.f_floor = updatef.f_floor;
                tb_finance.f_user = updatef.f_user;
                tb_finance.f_transaction = updatef.f_transaction;
                tb_finance.f_desc = updatef.f_desc;
                tb_finance.f_transactiontype = updatef.f_transactiontype;
                tb_finance.f_paymentmethod = updatef.f_paymentmethod;
                tb_finance.f_date = updatef.f_date;

                db.Entry(tb_finance).State = EntityState.Modified;
                db.SaveChanges();
                CalculateCurrentMonthProfit();
                TempData["success"] = "Transaction is successfully saved!";
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
            TempData["success"] = "Transaction is deleted.";
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
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            //var currentDate = DateTime.Now.AddMonths(-1);
            //var currentMonth = currentDate.Month;
            //var currentYear = currentDate.Year;

            // Retrieve the investors from the tb_investor table
            var investors = db.tb_investor.ToList().Where(t => t.i_active == "active");

            var financeData = db.tb_finance
                .Where(t => t.f_date.Month == currentMonth && t.f_date.Year == currentYear && t.f_floor != null)
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
            int i = 0;

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

                    var user = db.tb_user.Find(investor.i_user);
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

                    if(i == 0)
                    {
                        ShareTransaction();
                        i++;
                    }
                    
                }
            }

            // Save the changes to the database
            db.SaveChanges();
        }

        public void ShareTransaction()
        {
            var uid = Convert.ToInt32(Session["id"]);
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var lastMonth = currentMonth - 1;
            var lastYear = currentYear;

            if (lastMonth == 0)
            {
                lastMonth = 12;
                lastYear--;
            }

            var lastYearMonth = $"{lastYear}-{lastMonth}";
            var lastMonthProfit = db.tb_profit
                .Where(p => p.p_month == lastYearMonth)
                .GroupBy(p => p.p_investor)
                .Select(g => new
                {
                    InvestorId = g.Key,
                    Profit = g.Sum(p => p.p_profit)
                })
                .ToList();

            bool check = true;
            double ori_profit = 0;

            foreach (var profitEntry in lastMonthProfit)
            {
                var investorId = profitEntry.InvestorId;
                var profit = profitEntry.Profit;
                var investor = db.tb_investor.Find(investorId);
                var user = db.tb_user.Find(investor.i_user);
                string name = user.u_username;

                var financeEntry = new tb_finance
                {
                    f_floor = null, // Modify as per your requirement
                    f_date = DateTime.Now, // Set the date to the current date
                    f_transaction = profit, // Set the profit as the transaction amount
                    f_transactiontype = "Outflow", // Set the transaction type as "Outflow"
                    f_paymentmethod = "Bank", // Modify as per your requirement
                    f_user = uid, // Modify as per your requirement
                    f_desc = $"Share {name}" // Modify the description as per your requirement
                };

                // Add the finance entry to the table
                db.tb_finance.Add(financeEntry);

                if (check)
                {
                    ori_profit = profit / investor.i_amount * 50000;
                    check = false;
                }
            }

            var partners = new List<string> { "Anas", "Nizam", "Aiezad" };
            foreach (var partner in partners)
            {
                var financeEntry = new tb_finance
                {
                    f_floor = null, // Modify as per your requirement
                    f_date = DateTime.Now, // Set the date to the current date
                    f_transaction = ori_profit, // Set the profit as the transaction amount
                    f_transactiontype = "Outflow", // Set the transaction type as "Outflow"
                    f_paymentmethod = "Bank", // Modify as per your requirement
                    f_user = uid, // Modify as per your requirement
                    f_desc = $"Share {partner}" // Modify the description as per your requirement
                };

                // Add the finance entry to the table
                db.tb_finance.Add(financeEntry);
            }
            // Save the ch anges to the database
            db.SaveChanges();
        }

        //public void FloorGaji(int uid, double amount)
        //{
        //    var floors = db.tb_floor;
        //    foreach (var floor in floors)
        //    {
        //        var id = floor.fl_id;

        //        var financeEntry = new tb_finance
        //        {
        //            f_floor = id, // Modify as per your requirement
        //            f_date = DateTime.Now, // Set the date to the current date
        //            f_transaction = 200, // Set the profit as the transaction amount
        //            f_transactiontype = "Outflow", // Set the transaction type as "Outflow"
        //            f_paymentmethod = "Bank", // Modify as per your requirement
        //            f_user = uid, // Modify as per your requirement
        //            f_desc = $"Gaji Anas" // Modify the description as per your requirement
        //        };
        //    }
        //}
    }
}
