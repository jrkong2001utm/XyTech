using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XyTech.Attributes;
using XyTech.Models;

namespace XyTech.Controllers
{
    [CustomAuthorize]
    public class HomeController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();
        public ActionResult Index()
        {
            ViewBag.tenant = db.tb_tenant.ToList();
            ViewBag.attendance = db.tb_attendance.ToList();
            ViewBag.finance = db.tb_finance.ToList();
            ViewBag.floor = db.tb_floor.ToList();
            ViewBag.inventory = db.tb_inventory.ToList();
            ViewBag.investor = db.tb_investor.ToList();
            ViewBag.landlord = db.tb_landlord.ToList();
            ViewBag.room = db.tb_room.ToList();
            ViewBag.user = db.tb_user.ToList();
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