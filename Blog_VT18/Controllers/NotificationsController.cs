using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog_VT18.Models;

namespace Blog_VT18.Controllers
{
    public class NotificationsController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private RepositoryManager manager = new RepositoryManager();

        // GET: Notifications
        public ActionResult Index()
        {
            return View(manager.db.Notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = manager.db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }
        

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = manager.db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = manager.db.Notifications.Find(id);
            manager.db.Notifications.Remove(notification);
            manager.db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                manager.db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
