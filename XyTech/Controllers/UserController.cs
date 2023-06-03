﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XyTech.Models;

namespace XyTech.Controllers
{
    public class UserController : Controller
    {
        private Entities db = new Entities();

        // GET: User
        public ActionResult Index()
        {
            return View(db.tb_user.ToList());
        }

        // GET: User/Details/5
        public ActionResult Details(string id)
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
        public ActionResult Create([Bind(Include = "u_username,u_pwd,u_email,u_phone,u_usertype")] tb_user tb_user)
        {
            if (ModelState.IsValid)
            {
                db.tb_user.Add(tb_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(string id)
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
        public ActionResult Edit([Bind(Include = "u_username,u_pwd,u_email,u_phone,u_usertype")] tb_user tb_user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(string id)
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

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tb_user tb_user = db.tb_user.Find(id);
            db.tb_user.Remove(tb_user);
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
    }
}