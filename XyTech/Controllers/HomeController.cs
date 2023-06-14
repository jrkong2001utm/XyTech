using Microsoft.Ajax.Utilities;
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
            ViewBag.weektenant = db.tb_tenant
                .Where(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3))
                .ToList();

            ViewBag.tenant = db.tb_tenant.Where(t => t.t_indate.Day >= DateTime.Today.Day && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3)).ToList();
            ViewBag.landlord = db.tb_landlord.Where(l => l.l_due <= DateTime.Today && l.l_active == "1").ToList();
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.lenroom = db.tb_room.Count(r => r.r_availability == 1 && r.r_active == "active");
            ViewBag.totalroom = db.tb_room.Count(r => r.r_active == "active");

            ViewBag.Month = DateTime.Today.ToString("MMMM");
            ViewBag.Year = DateTime.Today.Year;

            ViewBag.room = db.tb_room.Where(r => r.r_availability == 1 && r.r_active == "active").ToList();
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_indate.Day >= DateTime.Today.Day && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
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
            return View();
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