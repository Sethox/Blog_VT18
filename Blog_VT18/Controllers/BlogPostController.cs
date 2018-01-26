using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity;

namespace Blog_VT18.Controllers
{
    public class BlogPostController : BaseController
    {
        // Index sidan läser in en lista av befintliga blogposts
        public ActionResult Index()
        {
            //Kom ihåg att inkludera kategorier
            List<BlogPost> posts = db.BlogPosts.OrderByDescending(x=>x.ID).Include(Z=>Z.From).ToList();

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
        public ActionResult Create(string Create)
        {
            //Kom ihåg att lägga in kategorier
            //Categories category = db.Categories.Single(x => x.Name == Category);
            //newPost.Category = category;
            
            //Skapar en ny blogpost
            BlogPost newPost = new BlogPost();

            //Tilldelar den nya posten med värdet ur vår parameter
            newPost.Content = Create;

            //Identifierar användaren - Plockar Id från current user
            var DennaUser = User.Identity.GetUserId();

            //Sätter ett värde ifrån user id, där user id stämmer överrens 
            newPost.From = db.Users.Single(x=> x.Id == DennaUser);

            //En blogpost läggs till i vår context
            db.BlogPosts.Add(newPost);

            //Sparar ändringar i databasen
            db.SaveChanges();

            //Uppdaterar sidan genom att köra index-metoden igen
            return RedirectToAction("Index");
        }
    }
}