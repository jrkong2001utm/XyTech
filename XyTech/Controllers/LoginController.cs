using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (TempData.Count > 0)
                ViewBag.Message = TempData["Message"].ToString();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(login objchk)
        {
            if (ModelState.IsValid)
            {
                using (db_XyTechEntities db = new db_XyTechEntities())
                {
                    var obj = db.tb_user.Where(a => a.u_username.Equals(objchk.u_username) && a.u_pwd.Equals(objchk.u_pwd)).FirstOrDefault();

                    if (obj != null)
                    {
                        Session["id"] = obj.u_id.ToString();
                        Session["u_username"] = obj.u_username.ToString();
                        Session["usertype"] = obj.u_usertype.ToString();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect Username or Password");
                    }
                }
            }
            return View(objchk);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}