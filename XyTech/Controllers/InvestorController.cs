using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class InvestorController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Investor
        public ActionResult Index()
        {
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_outdate <= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            var tb_investor = db.tb_investor.Include(t => t.tb_user);
            return View(tb_investor.ToList());
        }

        // GET: Investor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investor tb_investor = db.tb_investor.Find(id);
            if (tb_investor == null)
            {
                return HttpNotFound();
            }
            return View(tb_investor);
        }

        // GET: Investor/Create
        public ActionResult Create()
        {
            ViewBag.Username = TempData["InvestorUsername"] as string;

            using (var context = new XyTech.Models.db_XyTechEntities())
            {
                var username = ViewBag.Username as string;
                var user = context.tb_user.FirstOrDefault(u => u.u_username == username);
                if (user != null)
                {
                    var userList = context.tb_user.Select(u => new { u.u_id, u.u_username }).ToList();
                    ViewBag.i_user = new SelectList(userList, "u_id", "u_username", user.u_id);
                }
            }

            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "i_id,i_user,i_lot,i_amount,i_active")] tb_investor tb_investor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = db.tb_user.Find(tb_investor.i_user);
                    var u_email = user.u_email;
                    var u_name = user.u_username;
                    var u_pwd = user.u_pwd;
                    email(u_email, u_name, u_pwd);

                    var userId = Convert.ToInt32(Session["id"]);
                    var financeTransaction = new tb_finance
                    {
                        f_floor = null, // Modify as per your requirement
                        f_date = DateTime.Now, // Set the finance transaction date to current date
                        f_transaction = tb_investor.i_amount, // Set the transaction amount as per your requirement
                        f_transactiontype = "Inflow", // Set the transaction type as per your requirement
                        f_paymentmethod = "Bank",
                        f_user = userId,
                        f_desc = "Investor " + u_name
                    };

                    //Hash Password

                    db.tb_finance.Add(financeTransaction);
                    db.tb_investor.Add(tb_investor);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(e => e.ValidationErrors)
                        .Select(e => e.ErrorMessage);

                    // Log or handle the validation errors as needed
                    foreach (var errorMessage in errorMessages)
                    {
                        ModelState.AddModelError("", errorMessage);
                    }
                }
            }

            ViewBag.Username = TempData["InvestorUsername"] as string;
            using (var context = new XyTech.Models.db_XyTechEntities())
            {
                var username = ViewBag.Username as string;
                var user = context.tb_user.FirstOrDefault(u => u.u_username == username);
                if (user != null)
                {
                    var userList = context.tb_user.Select(u => new { u.u_id, u.u_username }).ToList();
                    ViewBag.i_user = new SelectList(userList, "u_id", "u_username", user.u_id);
                }
            }

            return View(tb_investor);
        }






        // GET: Investor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investor tb_investor = db.tb_investor.Find(id);
            if (tb_investor == null)
            {
                return HttpNotFound();
            }
            ViewBag.i_user = new SelectList(db.tb_user, "u_id", "u_username", tb_investor.i_user);
            return View(tb_investor);
        }

        // POST: Investor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "i_id,i_user,i_lot,i_amount,i_active")] tb_investor tb_investor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_investor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.i_user = new SelectList(db.tb_user, "u_id", "u_username", tb_investor.i_user);
            return View(tb_investor);
        }

        // GET: Investor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_investor tb_investor = db.tb_investor.Find(id);
            if (tb_investor == null)
            {
                return HttpNotFound();
            }
            return View(tb_investor);
        }

        // POST: Investor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_investor tb_investor = db.tb_investor.Find(id);
            db.tb_investor.Remove(tb_investor);
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

        public void email(string u_email, string username, string password)
        {
            using (db_XyTechEntities dc = new db_XyTechEntities())
            {

                    var verifyUrl = "/Login";
                    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                    var fromEmail = new MailAddress("akmalrentalsjb@gmail.com", "Akmal Rentals");
                    var toEmail = new MailAddress(u_email);
                    var fromEmailPassword = "wiuiwiwnutfzbqrm";

                    string subject = "Akmal Rentals User Account";
                    string body = "Dear " + username +
                        ",<br/><br/>Your account has been created successfully. Here are your login credentials:<br/><br/>" +
                        $"Username: {username}<br/>Password: {password}" +
                        $"<br/><br/>Please keep these credentials secure and do not share them with anyone.<br/><br/>" +
                        $"You can log in to your account by clicking on the following link: " +
                        $"<a href='{link}'>Log In</a><br/><br/>Thank you for joining Akmal Rentals.\";";


                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = fromEmail;
                        mail.To.Add(toEmail);
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;
                        //mail.Attachments.Add(new Attachment("C:\\file.zip"));

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword);
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
            }
        }


    }
}
