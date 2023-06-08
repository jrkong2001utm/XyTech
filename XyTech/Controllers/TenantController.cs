using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using XyTech.Attributes;
using XyTech.Models;

namespace XyTech.Controllers
{
    //[CustomAuthorize]
    public class TenantController : Controller
    {
        private db_XyTechEntities db = new db_XyTechEntities();

        // GET: Tenant
        public ActionResult Index()
        {
            var tb_tenant = db.tb_tenant.Include(t => t.tb_room);
            return View(tb_tenant.ToList());
        }
        [HttpGet]
        public ActionResult getTenantData() 
        {
            //var tenantData = from tenant in db.tb_tenant select new { tenant.t_name, tenant.t_ic, tenant.t_phone, tenant.t_indate, tenant.t_outdate, tenant.t_outstanding, tenant.t_paymentstatus, tenant.tb_room.r_floor}; // Retrieve data from the tb_tenant table
            var data = db.tb_tenant.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet); // Return the data as JSON; 
        }

        // GET: Tenant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }
            return View(tb_tenant);
        }

        // GET: Tenant/Create
        public ActionResult Create()
        {
            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_floor");
            return View();
        }

        // POST: Tenant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "t_id,t_name,t_ic,t_uploadic,t_contract,t_phone,t_emergency,t_indate,t_outdate,t_outsession,t_siri,t_outstanding,t_paymentstatus,t_room")] tb_tenant tb_tenant, HttpPostedFileBase icfile, HttpPostedFileBase contractfile)
        {
            if (ModelState.IsValid)
            {

                if (icfile != null && icfile.ContentLength > 0)
                {
                    if (icfile.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(icfile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/Icfile"), _FileName);
                        icfile.SaveAs(_path);
                        tb_tenant.t_uploadic = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_tenant);
                    }
                }

                if (contractfile != null && contractfile.ContentLength > 0)
                {
                    if (contractfile.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(contractfile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/Contractfile"), _FileName);
                        contractfile.SaveAs(_path);
                        tb_tenant.t_contract = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_tenant);
                    }
                }

                db.tb_tenant.Add(tb_tenant);
                db.SaveChanges();
                TempData["success"] = "Tenant saved successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_floor", tb_tenant.t_room);
            return View(tb_tenant);
        }

        // GET: Tenant/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }
            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_floor", tb_tenant.t_room);
            return View(tb_tenant);
        }

        // POST: Tenant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "t_id,t_name,t_ic,t_uploadic,t_contract,t_phone,t_emergency,t_indate,t_outdate,t_outsession,t_siri,t_outstanding,t_paymentstatus,t_room")] tb_tenant tb_tenant, HttpPostedFileBase icfile, HttpPostedFileBase contractfile)
        {
            if (ModelState.IsValid)
            {
                if (icfile != null && icfile.ContentLength > 0)
                {
                    System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/assets/images/Icfile"), tb_tenant.t_uploadic));
                    if (icfile.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(icfile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/Icfile"), _FileName);
                        icfile.SaveAs(_path);
                        tb_tenant.t_uploadic = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_tenant);
                    }
                }

                if (contractfile != null && contractfile.ContentLength > 0)
                {
                    if (icfile.ContentType.Contains("image"))
                    {
                        string _FileName = Path.GetFileName(contractfile.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Content/assets/images/Contractfile"), _FileName);
                        contractfile.SaveAs(_path);
                        tb_tenant.t_contract = _FileName;
                    }
                    else
                    {
                        ViewBag.Message = "Please choose image only.";
                        return View(tb_tenant);
                    }
                }

                db.Entry(tb_tenant).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Tenant updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_floor", tb_tenant.t_room);
            return View(tb_tenant);
        }

        // GET: Tenant/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }
            return View(tb_tenant);
        }

        // POST: Tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            db.tb_tenant.Remove(tb_tenant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetFile(string FileName)
        {
            string icfilePath = Server.MapPath("~/Content/assets/images/Icfile/" + FileName);
            string contractfilePath = Server.MapPath("~/Content/assets/images/Contractfile/" + FileName);

            if (System.IO.File.Exists(icfilePath))
            {
                return File(icfilePath, "image/png"); // Adjust the content type according to the actual file type
            }
            else
            {
                return HttpNotFound();
            }
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
