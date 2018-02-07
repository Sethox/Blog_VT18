﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Blog_VT18.Controllers {
    [Authorize(Roles = "Administrator")]
    public class RoleModelController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RepositoryManager manager;

        public RoleModelController() { this.manager = new RepositoryManager(); }

        // GET: RoleModel
        public ActionResult Index() {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(manager.db));
            var usrLst = manager.usrList();
            var remodelLst = new List<RoleModel>();
            foreach (var i in usrLst) {
                var m = userManager.GetRoles(i.Id).ToList().First();
                remodelLst.Add(new RoleModel { ID = i.Id, Name = i.UserName, Role = m, ByWhom = manager.usr });
            }
            return View(remodelLst.ToList());
        }

        // GET: RoleModel/Edit/5
        public ActionResult Edit(string id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var usr = db.Users.Find(id);
            RoleModel roleModel = new RoleModel { ID = manager.usr.Id, Name = manager.usr.UserName, Role = "Administrator", ByWhom = manager.usr };
            if (roleModel == null) {
                return HttpNotFound();
            }
            return View(roleModel);
        }

        // POST: RoleModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Role")] RoleModel roleModel) {
            if (ModelState.IsValid) {
                db.Entry(roleModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roleModel);
        }

        /* GET: RoleModel/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleModel roleModel = db.RoleModels.Find(id);
            if (roleModel == null) {
                return HttpNotFound();
            }
            return View(roleModel);
        }

        // GET: RoleModel/Create
        public ActionResult Create() {
            return View();
        }

        // POST: RoleModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Role")] RoleModel roleModel) {
            if (ModelState.IsValid) {
                db.RoleModels.Add(roleModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roleModel);
        }

        // GET: RoleModel/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleModel roleModel = db.RoleModels.Find(id);
            if (roleModel == null) {
                return HttpNotFound();
            }
            return View(roleModel);
        }

        // POST: RoleModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            RoleModel roleModel = db.RoleModels.Find(id);
            db.RoleModels.Remove(roleModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}