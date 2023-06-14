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
            ViewBag.countlandlord = db.tb_landlord.Count(l => l.l_due <= DateTime.Today && l.l_active == "1");
            ViewBag.counttenant = db.tb_tenant.Count(t => t.t_outdate <= DateTime.Today && (t.t_paymentstatus == 2 || t.t_paymentstatus == 3));

            var tb_tenant = db.tb_tenant.Include(t => t.tb_room);
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_tenant.ToList());
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

        [HttpPost]
        public ActionResult GetRoomNumbers(int floorId)
        {
            var roomNumbers = db.tb_room
             .Where(r => r.r_floor == floorId && r.r_availability == 1)
                .Select(r => new SelectListItem
                {
                    Value = r.r_id.ToString(),
                    Text = r.r_no.ToString()
                })
                .ToList();

            return Json(roomNumbers);
        }

        // GET: Tenant/Create
        public ActionResult Create(int id)
        {
            var room = db.tb_room.Find(id);
            ViewBag.RoomPrice = room.r_price;
            ViewBag.RoomID = room.r_id;

            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_no");
            var uniqueFloorIds = db.tb_room.Select(r => r.r_floor).Distinct().ToList();
            ViewBag.fl_id = new SelectList(uniqueFloorIds);
            return View();
        }

        // POST: Tenant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "t_id,t_name,t_ic,t_uploadic,t_contract,t_phone,t_emergency,t_indate,t_outdate,t_outsession,t_siri")] tb_tenant tb_tenant, HttpPostedFileBase icfile, HttpPostedFileBase contractfile)
        {
            // Retrieve the room price and room number from the form data
            var roomPrice = Request.Form["RoomPrice"];
            var RoomID = Request.Form["RoomID"];
            var method = Request.Form["PaymentMethod"];
            var d_amount = Request.Form["DepositAmount"];

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

                // Set the room price and room number in the tenant object
                tb_tenant.t_outstanding = Convert.ToDouble(roomPrice);
                tb_tenant.t_room = Convert.ToInt32(RoomID);
                tb_tenant.t_paymentstatus = 3;

                var room = db.tb_room.FirstOrDefault(r => r.r_id == tb_tenant.t_room);
                if (room != null)
                {
                    room.r_availability = 0;
                    //db.SaveChanges();

                    var userId = Convert.ToInt32(Session["id"]);
                    double amount = Convert.ToDouble(d_amount);
                    if (amount!=0 && !string.IsNullOrEmpty(method))
                    {

                        var financeTransaction = new tb_finance
                        {
                            f_floor = room.r_floor,
                            f_date = DateTime.Now,
                            f_transaction = amount,
                            f_transactiontype = "Inflow",
                            f_paymentmethod = method,
                            f_user = userId,
                            f_desc = "deposit " + tb_tenant.t_name + " " + room.r_no
                        };

                        db.tb_finance.Add(financeTransaction);
                        db.SaveChanges();
                    }
                }

                db.tb_tenant.Add(tb_tenant);
                db.SaveChanges();
                TempData["success"] = "Tenant saved successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.RoomPrice = roomPrice;
            ViewBag.RoomID = RoomID;
            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_no", tb_tenant.t_room);
            return View(tb_tenant);
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
            if (tenant.t_outstanding != amount && amount != 0)
            {
                tenant.t_paymentstatus = 2;
            }
            else if (tenant.t_outstanding == amount)
            {
                tenant.t_paymentstatus = 1;
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
            var uniqueFloorIds = db.tb_floor.Select(r => r.fl_id).Distinct().ToList();
            ViewBag.fl_id = new SelectList(uniqueFloorIds);
            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_no", tb_tenant.t_room);
            return View(tb_tenant);
        }

        // POST: Tenant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, tb_tenant updatetenant, HttpPostedFileBase icfile, HttpPostedFileBase contractfile)
        {
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            if (tb_tenant == null)
            {
                return HttpNotFound();
            }

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

                var previousRoom = db.tb_room.FirstOrDefault(r => r.r_id == tb_tenant.t_room);
                if (previousRoom != null)
                {
                    previousRoom.r_availability = 1;
                    db.SaveChanges();
                }
                var updateRoom = db.tb_room.FirstOrDefault(r => r.r_id == updatetenant.t_room);
                if (updateRoom != null)
                {
                    updateRoom.r_availability = 0;
                    db.SaveChanges();
                }


                tb_tenant.t_id = updatetenant.t_id;
                tb_tenant.t_name = updatetenant.t_name;
                tb_tenant.t_ic = updatetenant.t_ic;
                tb_tenant.t_phone = updatetenant.t_phone;
                tb_tenant.t_emergency = updatetenant.t_emergency;
                tb_tenant.t_indate = updatetenant.t_indate;
                tb_tenant.t_outdate = updatetenant.t_outdate;
                tb_tenant.t_outsession = updatetenant.t_outsession;
                tb_tenant.t_siri = updatetenant.t_siri;
                tb_tenant.t_room = updatetenant.t_room;

                db.Entry(tb_tenant).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Tenant updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.t_room = new SelectList(db.tb_room, "r_id", "r_no", tb_tenant.t_room);
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
            if (TempData.Count > 0)
            {
                ViewBag.Message = TempData["success"].ToString();
            }
            return View(tb_tenant);
        }

        // POST: Tenant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_tenant tb_tenant = db.tb_tenant.Find(id);
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/assets/images/Icfile"), tb_tenant.t_uploadic));
            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/assets/images/Contractfile"), tb_tenant.t_contract));
            var room = db.tb_room.FirstOrDefault(r => r.r_id == tb_tenant.t_room);
            if (room != null)
            {
                room.r_availability = 1;
                db.SaveChanges();
            }
            db.tb_tenant.Remove(tb_tenant);
            db.SaveChanges();
            TempData["success"] = "Tenant deleted.";
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
