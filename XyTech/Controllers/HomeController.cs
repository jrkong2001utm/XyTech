﻿using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XyTech.Attributes;
using XyTech.Models;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Net;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();
        public ActionResult Index()
        {
            int currentDay = DateTime.Today.Day;
            int currentDay2 = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();
            var tenants2 = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }

            if (currentDay2 > 23 && tenants2.Any(t => t.t_indate.Day < 7))
            {
                currentDay2 -= 30;
            }

            ViewBag.tenant = tenants
                .Where(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && t.t_indate.Month != DateTime.Today.Month && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3))
                .ToList();

            ViewBag.weektenant = tenants2.Where(t => t.t_indate.Day >= currentDay2 && t.t_indate.Month != DateTime.Today.Month && t.t_indate.Day < (currentDay2 + 7) && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3)).ToList();
            ViewBag.landlord = db.tb_landlord.Where(l => l.l_due <= DateTime.Today && l.l_active == "1").ToList();
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.lenroom = db.tb_room.Count(r => r.r_availability == 1 && r.r_active == "active");
            ViewBag.totalroom = db.tb_room.Count(r => r.r_active == "active");

            ViewBag.Month = DateTime.Today.ToString("MMMM");
            ViewBag.Year = DateTime.Today.Year;

            ViewBag.room = db.tb_room.Where(r => r.r_availability == 1 && r.r_active == "active").ToList();
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && t.t_indate.Month != DateTime.Today.Month && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            ViewBag.lentenant = db.tb_tenant.Count();
            ViewBag.leninvestor = db.tb_investor.Count(i => i.i_active == "active");

            ViewBag.JanInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 1).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.FebInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 2).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.MarInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 3).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.AprInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 4).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.MayInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 5).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.JunInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 6).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.JulInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 7).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.AugInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 8).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.SepInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 9).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.OctInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 10).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.NovInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 11).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.DecInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 12).Sum(f => (double?)f.f_transaction) ?? 0.0;

            ViewBag.JanOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 1).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.FebOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 2).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.MarOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 3).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.AprOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 4).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.MayOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 5).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.JunOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 6).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.JulOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 7).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.AugOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 8).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.SepOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 9).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.OctOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 10).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.NovOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 11).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.DecOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Year == DateTime.Today.Year && f.f_date.Month == 12).Sum(f => (double?)f.f_transaction) ?? 0.0;

            double totalInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Month == DateTime.Today.Month).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.TotalInflow = totalInflow;

            double totalOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Month == DateTime.Today.Month).Sum(f => (double?)f.f_transaction) ?? 0.0;
            ViewBag.TotalOutflow = totalOutflow;

            double totalProfit = totalInflow - totalOutflow;
            ViewBag.TotalProfit = totalProfit;

            double percentProfit = (totalInflow / (totalInflow + totalOutflow)) * 100;
            string formattedPercentProfit = percentProfit.ToString("F2");
            ViewBag.PercentProfit = formattedPercentProfit;

            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            foreach (var landlord in db.tb_landlord)
            {
                landlord.FloorList = new SelectList(db.tb_floor.Where(r => r.fl_active == "active"), "fl_id", "fl_bname");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Pay(int id, double amount, string method)
        {
            // Retrieve the tenant from the database
            var tenant = db.tb_tenant.Find(id);

            if (tenant == null)
            {
                // Tenant not found, handle the error accordingly
                return HttpNotFound();
            }

            var userId = Convert.ToInt32(Session["id"]);
            var room = db.tb_room.Find(tenant.t_room);

            var financeTransaction = new tb_finance
            {
                f_floor = room.r_floor, // Modify as per your requirement
                f_date = DateTime.Now, // Set the finance transaction date to current date
                f_transaction = amount, // Set the transaction amount as per your requirement
                f_transactiontype = "Inflow", // Set the transaction type as per your requirement
                f_paymentmethod = method,
                f_user = userId,
                f_desc = "sewa " + tenant.t_name + " " + room.r_no
            };

            db.tb_finance.Add(financeTransaction);
            db.SaveChanges();

            // Update the outstanding amount based on the payment amount
            if (tenant.t_outstanding > amount && amount != 0)
            {
                tenant.t_paymentstatus = 2;
            }
            else if (tenant.t_outstanding == amount)
            {
                tenant.t_paymentstatus = 1;
            }
            else if (tenant.t_outstanding < amount)
            {
                tenant.t_paymentstatus = 0;
            }
            tenant.t_outstanding -= amount;

            // Save the changes to the database
            db.Entry(tenant).State = EntityState.Modified;
            db.SaveChanges();

            // Set a success message to be displayed on the index page
            TempData["success"] = "Payment processed successfully!";

            // Redirect back to the index page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PayLandlord(int id, double amount, string method, int floor)
        {

            // Retrieve the landlord from the database
            var landlord = db.tb_landlord.Find(id);

            if (landlord == null)
            {
                // Landlord not found, handle the error accordingly
                return HttpNotFound();
            }

            var userId = Convert.ToInt32(Session["id"]);

            var financeTransaction = new tb_finance
            {
                f_floor = floor, // Modify as per your requirement
                f_date = DateTime.Now, // Set the finance transaction date to the current date
                f_transaction = amount, // Set the transaction amount as per your requirement
                f_transactiontype = "Outflow", // Set the transaction type as per your requirement
                f_paymentmethod = method,
                f_user = userId,
                f_desc = "sewa Owner - " + landlord.l_name
            };

            db.tb_finance.Add(financeTransaction);
            db.SaveChanges();
            //DateTime l_due = landlord.l_due;
            landlord.l_due = landlord.l_due.AddMonths(1);
            db.Entry(landlord).State = EntityState.Modified;
            db.SaveChanges();

            var landlords = db.tb_landlord
                        .Where(l => l.l_active != "0")
                        .Include(l => l.tb_bankname)
                        .OrderBy(l => l.l_due.Year)
                        .ToList();
            // Set a success message to be displayed on the index page
            TempData["success"] = "Payment processed successfully!";

            // Redirect back to the index page
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page. ";

            return View();
        }
    }
}