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
    [CustomAuthorize]
    public class UserController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: User
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            int currentDay = DateTime.Today.Day;
            var tenants = db.tb_tenant.ToList();

            if (currentDay < 7 && tenants.Any(t => t.t_indate.Day > 23))
            {
                currentDay += 30;
            }
            ViewBag.counttenant = tenants.Count(t => t.t_indate.Day >= (currentDay - 7) && t.t_indate.Month != DateTime.Today.Month && t.t_indate.Day < currentDay && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));
            
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(db.tb_user.ToList());
        }

        public ActionResult Profile(int? id)
        {
            if (TempData.Count > 0)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tb_user tb_user = db.tb_user.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }

            // Check if the request contains form data
            if (Request.Form.Count > 0)
            {
                string currentPassword = Request.Form["currentPassword"];
                string newPassword = Request.Form["newPassword"];
                string confirmPassword = Request.Form["confirmPassword"];

                // Check if the current password is correct
                if (currentPassword != tb_user.u_pwd)
                {
                    //ModelState.AddModelError("currentPassword", "Incorrect current password.");
                    ViewBag.Message = "Incorrect Current Password";
                    return View(tb_user);
                }

                // Check if the new password and confirm password match
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "New password and confirm password do not match.");
                }

                if (ModelState.IsValid)
                {
                    // Update the password in the database
                    tb_user.u_pwd = newPassword;
                    db.Entry(tb_user).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Password Changed Successfully";
                    // Redirect to the profile page with the updated password
                    return RedirectToAction("Profile", new { id = id });
                }
            }
            //ViewBag.Message = "Incorrect Current Password";
            return View(tb_user);
        }

        public ActionResult EditProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_user.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "u_id,u_username,u_pwd,u_email,u_phone,u_usertype,u_active")] tb_user tb_user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(tb_user.u_pwd))
                {
                    ModelState.Remove("u_pwd");
                }

                db.Entry(tb_user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Your Profile is successfully saved!";
                return RedirectToAction("Profile", new { id = tb_user.u_id });
            }
            return View(tb_user);
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_user.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "u_username,u_pwd,u_email,u_phone,u_usertype,u_active")] tb_user tb_user)
        {
            bool isUsernameExists = db.tb_user.Any(u => u.u_username == tb_user.u_username);

            if (isUsernameExists)
            {
                ModelState.AddModelError("u_username", "Username already exists.");
            }

            if (ModelState.IsValid)
            {
                db.tb_user.Add(tb_user);
                db.SaveChanges();
                TempData["success"] = "User is successfully saved!";

                if (tb_user.u_usertype == "Investor")
                {
                    // Redirect to the CreateInvestor action passing the user ID
                    TempData["InvestorUsername"] = tb_user.u_username;
                    return RedirectToAction("Create", "Investor", new { id = tb_user.u_id });
                }
                else
                {
                    return RedirectToAction("Index");
                }
               
            }

            return View(tb_user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_user.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "u_id,u_username,u_pwd,u_email,u_phone,u_usertype,u_active")] tb_user tb_user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(tb_user.u_pwd))
                {
                    ModelState.Remove("u_pwd");
                }

                db.Entry(tb_user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "User is successfully saved!";
                return RedirectToAction("Index");
            }
            return View(tb_user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_user.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            bool existsInInvestorTable = db.tb_investor.Any(p => p.i_user == id);
            bool existsInFinanceTable = db.tb_finance.Any(p => p.f_user == id);
            if (existsInInvestorTable || existsInFinanceTable)
            {
                TempData["success"] = "User is associated with other records and cannot be deleted.";
                return RedirectToAction("Index");
            }
            return View(tb_user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            tb_user tb_user = db.tb_user.Find(id);
            db.tb_user.Remove(tb_user);
            db.SaveChanges();
            TempData["success"] = "User is deleted.";
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
