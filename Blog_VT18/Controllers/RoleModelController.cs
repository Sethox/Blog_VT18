using System;
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

    // Only administators can access this feature
    [Authorize(Roles = "Administrator")]
    public class RoleModelController : Controller {
        private RepositoryManager manager;

        public RoleModelController() { this.manager = new RepositoryManager(); }

        // GET: RoleModel
        // Checkes if the database has some data and presents it if something exists
        public ActionResult Index() {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(manager.db));
            var usrLst = manager.usrList();
            var remodelLst = new List<RoleModel>();
            foreach (var i in usrLst) {
                var r = userManager.GetRoles(i.Id).ToList().First();
                remodelLst.Add(new RoleModel { ID = i.Id, Name = i.UserName, Role = r, ByWhom = manager.usr, IsEnabled = i.IsEnabled });
            }
            return View(remodelLst.ToList());
        }

        // GET: RoleModel/Edit/5
        // Modify and continues to Post
        public ActionResult Edit(string id) {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var usr = this.manager.db.Users.Find(id);
            var dl = new List<SelectListItem>();
            foreach(var i in manager.db.Roles.ToList()) {
                dl.Add(new SelectListItem { Value = i.Name, Text = i.Name });
            }
            RoleModel roleModel = new RoleModel { ID = manager.usr.Id, Name = manager.usr.UserName, Role = manager.getRole(id), ByWhom = manager.usr, Roles = dl };
            if (roleModel == null) return HttpNotFound();
            return View(roleModel);
        }

        // POST: RoleModel/Edit/5
        // Confirms and updates the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Role")] RoleModel roleModel) {
            if (ModelState.IsValid) {
                manager.UpdateRole(roleModel.ID, roleModel.Role);
                return RedirectToAction("Index");
            }
            return View(roleModel);
        }

        // GET: RoleModel/Delete/5
        public ActionResult Delete(string id) {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            manager.changeIsEnabled(id);
            return RedirectToAction("Index");
        }

        // Disposes the context and the this.manager.db's dispose
        protected override void Dispose(bool disposing) {
            if (disposing) this.manager.db.Dispose();
            base.Dispose(disposing);
        }
    }
}
