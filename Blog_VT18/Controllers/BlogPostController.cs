﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity;
using System.Net;

namespace Blog_VT18.Controllers {
    public class BlogPostController : BaseController {
        private RepositoryManager repositoryManager;

        public BlogPostController() { repositoryManager = new RepositoryManager(); }
        // Index sidan läser in en lista av befintliga blogposts
        public ActionResult Index() {
            //Kom ihåg att inkludera kategorier
            //var posts = db.BlogPosts.OrderByDescending(x => x.ID).Include(Z => Z.From).ToList();

            var cat = repositoryManager.CatList();
             
            
            ViewBag.MyViewBag = User.Identity.GetUserId();


            //Categories minKategori = new Categories();
            //minKategori.Category = null;
            //minKategori.Name = "Utbildning";
            //minKategori.ID = db.Categories.Count();
            //db.Categories.Add(minKategori);
            //db.SaveChanges();

            //Skickar oss till index och skickar med alla posts
            return View(cat);
        }

        // Creates a new blogpost and puts it through to the view
        public ActionResult Add(){
            var blogPost = new BlogPost(){             
                Hidden = false  
            };
            return View(blogPost);
        }


        // Accepts the blogpost whos have its values set in the View and sends it to the repositorie
        [HttpPost]
        public ActionResult Add(BlogPost blogPost, string id) {
            //ModelState.AddModelError("", "This is a global Message.");
            //ValidateEntry(entry);

            blogPost.From = repositoryManager.usr;


            if(ModelState.IsValid) {
                repositoryManager.newBlog(blogPost, id);
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO - Add a getBlogPost method
            BlogPost blogPost = repositoryManager.getBlogPost((int)id);


            if (blogPost == null)
            {
                return HttpNotFound();
            }

            return View(blogPost);
        }

        [HttpPost]
        public ActionResult Edit(BlogPost blogPost)
        {

            //TODO - create a changeBlog method in repository

            if (ModelState.IsValid)
            {
                repositoryManager.changeBlogPost(blogPost);

                return RedirectToAction("Index");
            }


            return View(blogPost);
        }

        public ActionResult Delete(int? Id) {

            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            repositoryManager.deleteBlogPost((int)Id);

            return RedirectToAction("Index");
        }

        public ActionResult Category(string id)
        {
            var catName1 = db.Categories.Single(x => x.ID.ToString().Equals(id));
            return View("Category", catName1);
        }

  

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