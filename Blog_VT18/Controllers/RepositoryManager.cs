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

namespace Blog_VT18.Controllers
{
    // A void class that is used to contact the database so the controllers does not contact the database/context directly
    public class RepositoryManager
    {
        // General ApplicationContext for all controllers
        public ApplicationDbContext db { private set; get; }
        // Variable for current user (logged in user)
        public ApplicationUser usr
        {
            get
            {
                var userID = HttpContext.Current.User.Identity.GetUserId();
                var currentUser = db.Users.Where(x => x.Id == userID).FirstOrDefault();
                return currentUser;
            }
        }

        public RepositoryManager() { db = new ApplicationDbContext(); }

        // Brings all catagories into a list
        public List<Categories> CatList(string id)
        {
            return db.Categories.Where(x => x.Category.ToString() == id).ToList();
        }

        public List<Categories> MainList()
        {
            return db.Categories.Where(x => x.Category == null).ToList();
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

        public List<TimeSuggestion> getInfo(int id)
        {
            var time = db.TimeSuggestions.Where(x => x.Meeting.ID == id).ToList();
            return time;
        }

        //public List<BlogPost> ListPosts(Categories category)
        //{
        //    var postList = db.BlogPosts.Where(x => x.Category.Equals(category.ID)).ToList();
        //    return postList;
        //}
        /// <summary>
        /// Updates the user currently logged in.
        /// </summary>
        /// <param name="user">The model to update in database.</param>

        public void setCurrentUser(ApplicationUser user)
        {
            usr.Name = user.Name;
            usr.UserName = user.UserName;
            usr.Email = user.Email;
            db.SaveChanges();
        }

        public ApplicationUser specificUser(string id)
        {
            return db.Users.Find(id);
        }

        public bool checkEmail(string email)
        {
            return db.Users.SingleOrDefault(x => x.Email == email).IsEnabled;
        }

        public void changeIsEnabled(string id)
        {
            var usr = db.Users.Find(id);
            if (usr.IsEnabled)
                usr.IsEnabled = false;
            else usr.IsEnabled = true;
            db.SaveChanges();
        }

        public void newCategory(Categories category, string id)
        {
            if (category != null)
            {
                category.Category = Int32.Parse(id);
                db.Categories.Add(category);
                db.SaveChanges();
            }
        }

        public void newBlog(BlogPost blogPost, string id)
        {
            Categories category = db.Categories.Single(x => x.ID.ToString().Equals(id));
            blogPost.Category = category;
            db.BlogPosts.Add(blogPost);
            db.SaveChanges();
        }

        public BlogPost getBlogPost(int? Id)
        {
            BlogPost blogPost = db.BlogPosts.Single(x => x.ID == Id);
            return blogPost;
        }

        public void changeBlogPost(BlogPost blogPost)
        {
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
            blogPost.Category = vP.Category;
            blogPost.From = usr;

            if (blogPost.Hidden == false)
            {
                blogPost.Hidden = true;
            }
            else
            {
                blogPost.Hidden = false;
            }

            db.SaveChanges();
        }

        public void deleteBlogPost(int? Id)
        {
            var bp = db.BlogPosts.Single(x => x.ID == Id);
            db.BlogPosts.Remove(bp);
            db.SaveChanges();
        }

        /// <summary>
        /// Disposing the classes properties.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
        }

        public List<Meeting> GetMeetings()
        {
            var meetings = db.Meetings.ToList();
            return meetings;
        }

        // Get list of calendar events
        public List<Meeting> getEventTimes()
        {
            if (db.Meetings.ToList().Count() > 0)
                return db.Meetings.ToList();
            return new List<Meeting>();
        }

        public bool checkIfMeetingExists(Meeting meeting)
        {
            var meetings = db.Meetings.Where(x => x.Booker.UserName == meeting.Booker.UserName && x.start_date.Date == meeting.start_date.Date && x.end_date.Date == meeting.end_date.Date).ToList();
            if (meetings != null)
                return true;
            else
                return false;
        }

        // Saves meeting
        public void setEventTime(Meeting meeting)
        {
           
                meeting.Booker = usr;
                db.Meetings.Add(meeting);
                db.SaveChanges();
            
        }

        public string getInvited(int Id)
        {
            var invited = db.InvitedToMeetings.Where(x => x.MeetingID == Id).Select(x => x.Invited).ToList();
            string invitedToMeeting = "";
            foreach (var item in invited) invitedToMeeting = invitedToMeeting + "\n" + item.Name;
            return invitedToMeeting;
        }

        public List<ApplicationUser> usrList()
        {
            return db.Users.ToList();
        }

        public List<string> GetAllRoles()
        {
            var roles = db.Roles.ToList();
            var roleList = new List<string>();
            foreach (var item in roles)
                roleList.Add(item.Name);
            return roleList;
        }

        /// <summary>
        /// Update the role for a specific user.
        /// </summary>
        /// <param name="id">Get the user from it's Id.</param>
        /// <param name="_role">The role it's going to change.</param>
        public void UpdateRole(string id, string _role)
        {
            UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(db));
            var user = _userManager.FindById(id);
            try
            {
                foreach (var role in user.Roles)
                {
                    if (role.UserId == id)
                    {
                        _userManager.RemoveFromRole(id, Convert.ToString(_userManager.GetRoles(id).ToList().First().ToString()));
                        _userManager.AddToRole(user.Id, _role);
                    }
                }
                db.SaveChanges();
            }
            catch (Exception) { }
        }

        public string getRole(string id)
        {
            var userRoleList = db.Users.Include(x => x.Roles).ToList();
            string roleId = "";
            foreach (var item in userRoleList)
            {
                if (item.Id == id)
                    foreach (var role in item.Roles)
                        roleId = role.RoleId;
            }
            var roleList = db.Roles.ToList();
            var theRole = "";
            foreach (var item in roleList)
                if (roleId == item.Id)
                    theRole = item.Name;
            return theRole;
        }
    }
}
