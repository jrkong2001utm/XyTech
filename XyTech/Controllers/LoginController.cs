using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using XyTech.Models;
using System.Web.Helpers;

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
                        if (obj.u_active == "active")
                        {
                            Session["id"] = obj.u_id.ToString();
                            Session["u_username"] = obj.u_username.ToString();
                            Session["usertype"] = obj.u_usertype.ToString();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "User Account is inactive. Please inform admin to activate.");
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect Username or Password");
                    }
                }
            }
            return View(objchk);
        }

        public ActionResult forgot()
        {
            return View();
        }

        [HttpPost]
        public ActionResult forgot(string u_username)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";

            using (db_XyTechEntities dc = new db_XyTechEntities())
            {
                var account = dc.tb_user.Where(a => a.u_username == u_username).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    
                    //send email
                    var verifyUrl = "/Login/ResetPassword/" + resetCode;
                    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                    string name = account.u_username;

                    var fromEmail = new MailAddress("akmalrentalsjb@gmail.com", "Akmal Rentals");
                    var toEmail = new MailAddress(account.u_email);
                    var fromEmailPassword = "wiuiwiwnutfzbqrm"; // Replace with actual password

                    string subject = "Reset Password";
                    string body = "Dear " + name +
                        ",<br/><br/>We got request for reset your account password. Please click on the below link to reset your password. The link will only be available for 7 days." +
                        "<br/><br/><a href=" + link + ">Reset Password link</a><br/><br/>If the link above is not working, please copy the URL link below and paste it in your browser. <br/><br/> Link : " +
                        link;

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
                    account.u_token = resetCode;
                    account.u_resetdate = DateTime.Now;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    return RedirectToAction("ResetLinkSent", new { email = account.u_email });
                }
                else
                {
                    message = "User not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetLinkSent(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (db_XyTechEntities dc = new db_XyTechEntities())
            {
                var user = dc.tb_user.Where(a => a.u_token == id).FirstOrDefault();
                if (user != null)
                {
                    if (user.u_resetdate != null && DateTime.Now.Subtract(user.u_resetdate.Value).TotalDays > 7)
                    {
                        return RedirectToAction("ExpiredResetLink");
                    }

                    resetpassword model = new resetpassword();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ExpiredResetLink");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(resetpassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (db_XyTechEntities dc = new db_XyTechEntities())
                {
                    var user = dc.tb_user.Where(a => a.u_token == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        //user.u_pwd = Crypto.Hash(model.NewPassword);
                        user.u_pwd = model.NewPassword;
                        user.u_token = null;
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        TempData["message"] = "New password updated successfully";
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.Message = message;
            return View(model);
        }

        public ActionResult ExpiredResetLink()
        {
            // Handle expired reset link
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}