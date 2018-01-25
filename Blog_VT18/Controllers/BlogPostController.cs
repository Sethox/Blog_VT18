using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using Blog_VT18.Models;

namespace Blog_VT18.Controllers
{
    public class BlogPostController : BaseController
    {
        // GET: BlogPost
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string content, string Category)
        {
            Categories category = db.Categories.Single(x => x.Name == Category);
            BlogPost newPost = new BlogPost();
            newPost.Category = category;
            newPost.Content = content;
                return View(newPost);
        }
    }
}