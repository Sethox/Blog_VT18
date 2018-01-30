using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity;
using System.Net;

namespace Blog_VT18.Controllers {
    public class BlogPostController : BaseController {
        private RepositoryManager repositoryManager;

        public BlogPostController() { repositoryManager = new RepositoryManager(); }
        // Index sidan läser in en lista av befintliga blogposts
        public ActionResult Index()
        {



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

        public ActionResult Add()
        {
            var blogPost = new BlogPost()
            {             
                Hidden = false  
            };      
            return View(blogPost);
        }

        [HttpPost]
        public ActionResult Add(BlogPost blogPost)
        {
            //ModelState.AddModelError("", "This is a global Message.");


            //ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                repositoryManager.newBlog(blogPost);
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    // TODO - Add a getBlogPost method
        //    //BlogPost blogPost = repositoryManager.getBlogPost((int)id);


        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
            
        //    return View(blogPost);
        //}

        //[HttpPost]
        //public ActionResult Edit(BlogPost blogPost)
        //{

        //    //TODO - create a changeBlog method in repository

        //    if (ModelState.IsValid)
        //    {
        //        //repositoryManager.changeBlog(blogPost);

        //        return RedirectToAction("Index");
        //    }


        //    return View(blogPost);
        //}

        //Här skapar vi en blogpost 
        //public ActionResult Create(string Create) {

        //    BlogPost hej = new BlogPost { Title = "Standard", Content = Create, Hidden = false, From =  repositoryManager.usr };
            
        //    hej.Content = Create;

        //    try
        //    {
        //        this.repositoryManager.newBlog(hej);
        //    }
        //    catch (Exception)
        //    {

        //        return RedirectToAction("Index");
        //    }

            
        //        return RedirectToAction("Index");
        //}
    }
}