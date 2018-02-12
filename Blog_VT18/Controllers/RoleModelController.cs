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
    [Authorize(Roles = "Administrator")]
    public class RoleModelController : Controller {
        private RepositoryManager manager;
        public RoleModelController() { manager = new RepositoryManager(); }

        // GET: RoleModel
        public ActionResult Index() {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(manager.db));
            var userList = manager.usrList();
            var roleModelList = new List<RoleModel>();
            foreach (var user in userList) {
                var role = userManager.GetRoles(user.Id).ToList().First();
                roleModelList.Add(new RoleModel { ID = user.Id, Name = user.UserName, Role = role, ByWhom = manager.usr, IsEnabled = user.IsEnabled });
            }
            return View(roleModelList.ToList());
        }

        // GET: RoleModel/Edit/5
        public ActionResult Edit(string id) {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var selectList = new List<SelectListItem>();
            foreach(var identityRole in manager.db.Roles.ToList()) {
                selectList.Add(new SelectListItem { Value = identityRole.Name, Text = identityRole.Name });
            }
            RoleModel roleModel = new RoleModel { ID = manager.usr.Id, Name = manager.usr.UserName, Role = manager.getRole(id), ByWhom = manager.usr, Roles = selectList };
            return View(roleModel);
        }

        // POST: RoleModel/Edit/5
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
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            manager.changeIsEnabled(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) manager.db.Dispose();
            base.Dispose(disposing);
        }
    }
}
