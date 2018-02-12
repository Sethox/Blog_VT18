using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Blog_VT18.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Blog_VT18.Controllers {
    public class RepositoryManager {
        public ApplicationDbContext db { private set; get; }
        public ApplicationUser usr {
            get {
                var userID = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = db.Users.Where(x => x.Id == userID).FirstOrDefault();
                return currentUser;
            }
        }

        public RepositoryManager() { this.db = new ApplicationDbContext(); }

        public List<Categories> CatList(string id) {
            var cat = db.Categories.Where(x => x.Category.ToString() == id).ToList();
            return cat;
            }

        public List<Categories> MainList() {
            var cat = db.Categories.Where(x => x.Category == null).ToList();
            return cat;
        }

        public Categories GetCategory(string id)
        {
            var catID = Int32.Parse(id);
            var cat = db.Categories.Single(x => x.ID == catID);
            return cat;
        }

        public string GetCategoryName(int id)
        {
            var cat = db.Categories.Single(x => x.ID == id).Name;
            return cat;
        }

        //public List<BlogPost> ListPosts(Categories category)
        //{
        //    var postList = db.BlogPosts.Where(x => x.Category.Equals(category.ID)).ToList();
        //    return postList;
        //}
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
            if(post != null) {
                this.db.BlogPosts.Add(post);
                this.db.SaveChanges();
            }
        }

        public ApplicationUser specificUser(string id) {
            return this.db.Users.Find(id);
        }

        public bool checkEmail(string email) {
            return this.db.Users.SingleOrDefault(x => x.Email == email).IsEnabled;
        }

        public void changeIsEnabled(string id) {
            var usr = this.db.Users.Find(id);
            if (usr.IsEnabled)
                usr.IsEnabled = false;
            else usr.IsEnabled = true;
            this.db.SaveChanges();
        }

        public void newCategory(Categories category, string id)
        {
            if (category != null) {
                category.Category = Int32.Parse(id);
                this.db.Categories.Add(category);
                this.db.SaveChanges();
            }
        }

        public void newBlog(BlogPost Create, string id) {
            //Kom ihåg att lägga in kategorier
            Categories category = db.Categories.Single(x => x.ID.ToString().Equals(id));
            Create.Category = category;
            // Creates new blog, updates database
            //BlogPost newPost = new BlogPost(Create);
            //En blogpost läggs till i vår context
            db.BlogPosts.Add(Create);
            //Sparar ändringar i databasen
            db.SaveChanges();
        }
            public BlogPost getBlogPost(int? Id) {
            BlogPost blogPost = db.BlogPosts.Single(x => x.ID == Id);
            return blogPost;
        }

        public void changeBlogPost(BlogPost blogPost) {
            var bp = db.BlogPosts.Where(x => x.ID == blogPost.ID).Single();
            var ny = db.BlogPosts.Where(x => x.ID == blogPost.ID).Single();
            ny = blogPost;
            ny.Category = bp.Category;
            ny.From = usr;
            db.BlogPosts.Remove(bp);
            db.BlogPosts.Add(ny);
            db.SaveChanges();
        }

        public void hidePost(BlogPost blogPost)
        {
            var vP = db.BlogPosts.Where(x => x.ID == blogPost.ID).Single();
            var hP = db.BlogPosts.Where(x => x.ID == blogPost.ID).Single();
            hP = blogPost;
            hP.Category = vP.Category;
            hP.From = usr;

            if (hP.Hidden == false)
            {
                hP.Hidden = true;
            }
            else
            {
                hP.Hidden = false;
            }

            db.SaveChanges();
        }

        public void deleteBlogPost(int? Id) {
            var bp = db.BlogPosts.Single(x => x.ID == Id);
            db.BlogPosts.Remove(bp);
            db.SaveChanges();
        }
            /// <summary>
            /// Disposing the classes properties.
            /// </summary>
            protected void Dispose(bool disposing) {
            if(disposing && this.db != null) {
                this.db.Dispose();
                this.db = null;
            }
        }
        public List<Meeting> GetMeetings()
        {
            var meetings = db.Meetings.ToList();
            return meetings;
        }

        // Getting EVERY calender event
        public List<Meeting> getEventTimes() {
            if(db.Meetings.ToList().Count() > 0)
                return db.Meetings.ToList();
                return new List<Meeting>();
        }
        // Saves Specific calender event
            public void setEventTime(Meeting Event_Date) {
            Event_Date.Booker = usr;
            db.Meetings.Add(Event_Date);
            db.SaveChanges();
        }

            public string getInvited(int Id) {
            var invited = db.InvitedToMeetings.Where(x => x.MeetingID == Id).Select(x => x.Invited).ToList();
            string z = "";
            foreach(var item in invited) z = z + "\n" + item.Name;
            return z;
        }

            public List<ApplicationUser> usrList() {
            return db.Users.ToList();
        }

        public List<string> GetAllRoles() {
            var roles = db.Roles.ToList();
            var roleList = new List<string>();
            foreach(var item in roles) {
                roleList.Add(item.Name);
            }
            return roleList;
        }

        /// <summary>
        /// Update the role for a specific user.
        /// </summary>
        /// <param name="id">Get the user from it's Id.</param>
        /// <param name="_role">The role it's going to change.</param>
        public void UpdateRole(string id, string _role) {
            UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
        new UserStore<ApplicationUser>(this.db));
            var user = _userManager.FindById(id);
            //string aL = _userManager.GetRoles(id).ToList().First().ToString();
            try {
                foreach(var role in user.Roles) {
                    if(role.UserId == id) {
                        _userManager.RemoveFromRole(id, Convert.ToString(_userManager.GetRoles(id).ToList().First().ToString()));
                        _userManager.AddToRole(user.Id, _role);
                    }
                }
                this.db.SaveChanges();
            } catch(Exception) { }
        }
        /*
        public void setRole(string id, string newRole) {
            var userRoleList = db.Roles.SingleOrDefault().Users;
            var role = userRoleList.SingleOrDefault(x => x.UserId == id);
            var roleList = db.Roles.ToList();
            var nR = db.Roles.Where(x => x.Name == newRole);
            foreach(var item in roleList) {
                if(item.Id == role.RoleId) {

                    userRoleList.Clear();
                    userRoleList.Add(new IdentityUserRole { UserId = id, RoleId = nR.First().Id });
                    //role.RoleId = newRole;
                }
            }
            db.SaveChanges();
        }*/

        public string getRole(string id) {
            var userRoleList = db.Users.Include(x => x.Roles).ToList();
            string roleId = "";
            foreach(var item in userRoleList) {
                if(item.Id == id)
                    foreach(var role in item.Roles) {
                        roleId = role.RoleId;
                    }
            }
            var roleList = db.Roles.ToList();
            var temp = "";
            foreach(var item in roleList) {
                if(roleId == item.Id)
                    temp = item.Name;
            }

            //var roleList = db.Roles.ToList();
            //IdentityUserRole role;
            //var temp = "";
            //foreach (var item in roleList)
            //{
            //    role = userRoleList.);
            //    foreach (var i in roleList)
            //    {
            //        if (i.Id == role.RoleId)
            //        {
            //            temp = i.Name;
            //        }
            //    }
            //}
            return temp;
        }
    }
}