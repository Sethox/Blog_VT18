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

        public ActionResult Create(string Create)
        {
            //Categories category = db.Categories.Single(x => x.Name == Category);
            //newPost.Category = category;
            BlogPost newPost = new BlogPost();
            newPost.Content = Create;
                return View(newPost);
        }
    }
}