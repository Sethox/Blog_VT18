using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity;

namespace Blog_VT18.Controllers {
    public class BlogPostController : BaseController {
        private RepositoryManager repositoryManager;

        public BlogPostController() { repositoryManager = new RepositoryManager(); }
        // Index sidan läser in en lista av befintliga blogposts
        public ActionResult Index() {
            //Kom ihåg att inkludera kategorier
            List<BlogPost> posts = db.BlogPosts.OrderByDescending(x => x.ID).Include(Z => Z.From).ToList();

            //Categories minKategori = new Categories();
            //minKategori.Category = null;
            //minKategori.Name = "Utbildning";
            //minKategori.ID = db.Categories.Count();
            //db.Categories.Add(minKategori);
            //db.SaveChanges();

            //Skickar oss till index och skickar med alla posts
            return View(posts);
        }

        //Här skapar vi en blogpost 
        public ActionResult Create(BlogPost Create) {
            this.repositoryManager.newBlog(Create);
            return RedirectToAction("Index");
        }
    }
}