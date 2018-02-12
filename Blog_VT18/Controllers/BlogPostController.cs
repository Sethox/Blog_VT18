using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity;
using System.Net;
using System.IO;

namespace Blog_VT18.Controllers {

    [System.Web.Mvc.Authorize(Roles = "Administrator,User")]
    public class BlogPostController : BaseController {
        private RepositoryManager repositoryManager;

        public BlogPostController() { repositoryManager = new RepositoryManager(); }
        // Index reads a list of all the blogposts that exists in the database
        public ActionResult Index() {
            if(TempData["Error"] != null) ModelState.AddModelError("", TempData["Error"].ToString());
            ViewBag.Message = "guguug";
            ViewBag.MyViewBag = User.Identity.GetUserId();
            // Send the object through the view
            return View(repositoryManager.MainList());
        }

        // Creates a new blogpost and puts it through to the view
        public ActionResult Add(string id) {
            var idet = id;
            var posts = repositoryManager.db.BlogPosts.OrderByDescending(x => x.ID).Where(x => x.Category.ID.ToString() == id).Include(Z => Z.From).ToList();
            ViewBag.PostViewBag = posts;
            return View(new BlogPost { Hidden = false, From = repositoryManager.usr } );
        }

        // Accepts the blogpost whos have its values set in the View and sends it to the repositorie
        [HttpPost]
        public ActionResult Add(BlogPost blogPost, string id, HttpPostedFileBase upload) {
            blogPost.Category = repositoryManager.GetCategory(id);
            blogPost.From = repositoryManager.usr;
            if (ModelState.IsValid) {
                if (upload == null) repositoryManager.newBlog(blogPost, id);
                else if (upload != null && upload.ContentLength < 25000000) {
                        blogPost.Filename = upload.FileName;
                        blogPost.ContentType = upload.ContentType;
                        using (var reader = new BinaryReader(upload.InputStream)) { blogPost.File = reader.ReadBytes(upload.ContentLength); }
                        repositoryManager.newBlog(blogPost, id);
                }
                else if (upload.ContentLength > 25000000) ModelState.AddModelError("", "Too large");
            }
            return RedirectToAction("Add", "BlogPost", new { id = id });
        }

        // Download as a feature/function when something is uploaded
        public ActionResult Download(int id) {
            return File(this.repositoryManager.db.BlogPosts.Find(id).File, this.repositoryManager.db.BlogPosts.Find(id).ContentType, this.repositoryManager.db.BlogPosts.Find(id).Filename);
        }

        // Display a specific post
        public ActionResult Show(int? id){
            var thePicture = this.repositoryManager.db.BlogPosts.Single(x => x.ID == id);
            if (thePicture.File is null)
                return File("FinnsEj", ".jpg");
            return File(thePicture.File, thePicture.ContentType);
        }

        // Modify a post
        [HttpGet]
        public ActionResult Edit(int? id) {
            if(id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            // TODO - Add a getBlogPost method
            BlogPost blogPost = repositoryManager.getBlogPost((int)id);
            if(blogPost == null) return HttpNotFound();
            return View(blogPost);
        }

        // Confirms the modification (from Get)
        [HttpPost]
        public ActionResult Edit(BlogPost blogPost, string id, HttpPostedFileBase upload) {
            //TODO - create a changeBlog method in repository
            if (upload != null && upload.ContentLength > 0 && upload.ContentLength < 25000000) {
                blogPost.Filename = upload.FileName;
                blogPost.ContentType = upload.ContentType;
                using (var reader = new BinaryReader(upload.InputStream)) { blogPost.File = reader.ReadBytes(upload.ContentLength); }
            }
            if (ModelState.IsValid) {
                repositoryManager.changeBlogPost(blogPost);
                return RedirectToAction("Add", "BlogPost" , new { id = blogPost.Category.ID });
            }
            return View(blogPost);
        }

        // Hides a post - TODO not complete
        public ActionResult HidePost(int? Id) {
            if(Id != null) {
                var blogPost = repositoryManager.getBlogPost(Id);
                var id = blogPost.Category.ID;
                repositoryManager.hidePost(blogPost);
                return RedirectToAction("Add", "BlogPost", new { id = id });
            } else {
                TempData["Error"] = "Something wrong happened..Try again.";
                return RedirectToAction("Index");
            }
        }

        // Delete a posts
        public ActionResult Delete(int? Id) {
            if(Id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            repositoryManager.deleteBlogPost((int)Id);
            return RedirectToAction("Index");
        }

        // Catches a specific category
        public ActionResult Category(string id) {
            var subCats = repositoryManager.CatList(id);
            var intId = Int32.Parse(id);
            ViewBag.Message = repositoryManager.GetCategoryName(intId);
            return View("Category", subCats);
        }

        // Catches a list of subcategories
        public List<Categories> subCategories(string id) {
            var subCats = repositoryManager.CatList(id);
            return subCats;
        }

        // Creates a category
        [HttpGet]
        public ActionResult CreateCategory(int id) {
            ViewBag.CategoryName = repositoryManager.GetCategoryName(id);
            var category = new Categories() {
                Category = id
            };
            return View(category);
        }

        // Confirms the new category created from Get
        [HttpPost]
        public ActionResult CreateCategory(Categories category, string id) {
            if(ModelState.IsValid) {
                repositoryManager.newCategory(category, id);
                return RedirectToAction("Category", new { id = id });
            }
            return RedirectToAction("Category", new { id = id });
        }

        // Lists all post of a specific subcatagory
        public ActionResult ListPost(IEnumerable<Categories> categories) {
            var postList = new List<BlogPost>();
            for(int i = 0; i < categories.Count(); i++) {
                var post = this.repositoryManager.db.BlogPosts.Where(x => x.Category == categories);
                foreach(BlogPost item in post)
                    postList.Add(item);
            }
            return View(postList);
        }

        // Catches a specific blogpost
        public ActionResult ViewBlogPost(int? id) {
            var blogPost = repositoryManager.getBlogPost(id);
            return View(blogPost);
        }
    }
}