﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Blog_VT18.Models;
using System.Collections.Generic;

namespace Blog_VT18.Controllers {
    public class RepositoryManager {
        ApplicationDbContext db;
        public ApplicationUser usr { get {
                var userID = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = db.Users.Where(x => x.Id == userID).FirstOrDefault();
                return currentUser;
            } }

        public RepositoryManager() { this.db = new ApplicationDbContext(); }

        /// <summary>
        /// Updates the user currently logged in.
        /// </summary>
        /// <param name="modifyUsr">The model to update in database.</param>
        public void setCurrentUser(ApplicationUser modifyUsr) {
            this.usr.Name = modifyUsr.Name;
            this.usr.UserName = modifyUsr.UserName;
            this.usr.Email = modifyUsr.Email;
            this.db.SaveChanges();
        }

        /// <summary>
        /// This creates a new catagory.
        /// </summary>
        /// <param name="post">This is the model to update the database.</param>
        public void newCatagory(BlogPost post) {
            if (post != null) {
                this.db.BlogPosts.Add(post);
                this.db.SaveChanges();
            }
        }

        public void newBlog(BlogPost Create) {
            //Kom ihåg att lägga in kategorier
            //Categories category = db.Categories.Single(x => x.Name == Category);
            //newPost.Category = category;

            // Creates new blog, updates database
            BlogPost newPost = new BlogPost(Create);

            //En blogpost läggs till i vår context
            db.BlogPosts.Add(newPost);

            //Sparar ändringar i databasen
            db.SaveChanges();
        }

        /// <summary>
        /// Disposing the classes properties.
        /// </summary>
        protected void Dispose(bool disposing) {
            if (disposing && this.db != null) {
                this.db.Dispose();
                this.db = null;
            }
        }
        public List<Meeting> GetMeetings()
        {
            var meetings = db.Meetings.ToList();

            return meetings;
        }
    }
}