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
            DateTime oneWeekBefore = DateTime.Today.AddDays(-7);
            DateTime currentDate = DateTime.Today;

            ViewBag.weektenant = db.tb_tenant
                .Where(t => t.t_outdate >= oneWeekBefore && t.t_outdate <= currentDate && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3))
                .ToList();

            ViewBag.tenant = db.tb_tenant.Where(t => t.t_outdate >= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3)).ToList();
            ViewBag.landlord = db.tb_landlord.Where(l => l.l_due <= DateTime.Today && l.l_active == "1").ToList();
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.lenroom = db.tb_room.Count(r => r.r_availability == 1);

            //var tb_room = db.tb_room.Include(t => t.tb_floor).Where(p => p.r_active.Equals("active"));
            //ViewBag.room = tb_room;
            ViewBag.Month = DateTime.Today.ToString("MMMM");

            ViewBag.room = db.tb_room.Where(r => r.r_availability == 1).ToList();
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_outdate <= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            ViewBag.lentenant = db.tb_tenant.Count();
            ViewBag.leninvestor = db.tb_investor.Count(i => i.i_active == "active");

            double totalInflow = db.tb_finance.Where(f => f.f_transactiontype == "Inflow" && f.f_date.Month == DateTime.Today.Month).Sum(f => f.f_transaction);
            ViewBag.TotalInflow = totalInflow;

            double totalOutflow = db.tb_finance.Where(f => f.f_transactiontype == "Outflow" && f.f_date.Month == DateTime.Today.Month).Sum(f => f.f_transaction);
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