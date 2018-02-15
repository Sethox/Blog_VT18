using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHTMLX.Scheduler;
using DHTMLX.Common;
using DHTMLX.Scheduler.Data;
using DHTMLX.Scheduler.Controls;
using Blog_VT18.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Collections;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace Scheduler.SignalR.Sample {
    [HubName("schedulerHub")]

    public class SchedulerHub : Hub { public void Send(string update) { this.Clients.All.addMessage(update); } } }


namespace Blog_VT18.Controllers {
    [System.Web.Mvc.Authorize(Roles = "Administrator,User")]
    public class CalendarController : BaseController {
        public RepositoryManager manager { get; set; }
        public CalendarController() { manager = new RepositoryManager(); }

        public ActionResult Index() {
            //Being initialized in that way, scheduler will use CalendarController.Data as a the datasource and CalendarController.Save to process changes
            var scheduler = new DHXScheduler(this);
            scheduler.InitialDate = new DateTime();
            scheduler.Extensions.Add(SchedulerExtensions.Extension.LiveUpdates);
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            return View(scheduler);
        }

        // This loads when index loads up, if something exists in the database, it loads up from the database with this method (only once)
        public ContentResult Data() {
            List<Meeting> calendar = manager.GetMeetings();
            List<Meeting> List = new List<Meeting>();
            List<InvitedToMeetings> UserList = new List<InvitedToMeetings>();

            foreach (var item in calendar) {
                string fyllMig = "";
                var nyList = this.manager.db.TimeSuggestions.Where(x => x.Meeting.ID == item.ID & x.Accepted == false & x.Denied == false).Select(x => x.Invited.Name).ToList();
                var acceptedList = this.manager.db.TimeSuggestions.Where(x => x.Meeting.ID == item.ID & x.Accepted == true).Select(x => x.Invited.Name).ToList();
                var deniedList = this.manager.db.TimeSuggestions.Where(x => x.Meeting.ID == item.ID & x.Denied == true).Select(x => x.Invited.Name).ToList();
                var listan = manager.getInvited(item.ID);
                if (nyList.Count() <= 0) {
                    nyList.Add("ingen");
                }
                foreach (var spot in nyList) {
                    fyllMig = fyllMig + "\n" + spot;
                }
                fyllMig = fyllMig + "\n \n Accepted";
                foreach (var spot in acceptedList) {
                    fyllMig = fyllMig + "\n" + spot;
                }
                fyllMig = fyllMig + "\n \n Denied";
                foreach (var spot in deniedList) {
                    fyllMig = fyllMig + "\n" + spot;
                }
                var aEvent = new Meeting {
                    ID = item.ID,
                    text = item.text + " \nBooked by: " + item.Booker.Name + "\nWaiting for response: " + fyllMig,
                    start_date = item.start_date,
                    end_date = item.end_date
                };
                List.Add(aEvent);
            }
            return (ContentResult) new SchedulerAjaxData(List);
        }

        // Saves a post, this method get called after the popup has been confirm to "save" a post
        public ContentResult Save(int? id, FormCollection actionValues) {
            var calendar = manager.getEventTimes();
            var action = new DataAction(actionValues);
            try {
                var changedEvent = (Meeting)DHXEventsHelper.Bind(typeof(Meeting), actionValues);
                switch (action.Type) {
                    case DataActionTypes.Insert:
                        manager.setEventTime(changedEvent);
                        break;
                    case DataActionTypes.Delete:
                        // For Delete
                        break;
                    default:
                        // For Update
                        break;
                }
            } catch { action.Type = DataActionTypes.Error; }
            return (ContentResult)new AjaxSaveResponse(action);
        }

        // Sends a time suggestion - Todo Need to finish
        public ActionResult SendTimeSuggestion() {
            TimeSuggestion Suggestion = new TimeSuggestion();
            ViewBag.Me = User.Identity.GetUserId();
            ApplicationUser user = this.manager.db.Users.Find(User.Identity.GetUserId());
            var list = new List<ApplicationUser> { user };
            Suggestion.Invited = user;
            var invitedList = user;
            var listItems = new List<ApplicationUser> {
            };
            foreach (var item in this.manager.db.Users) {
                listItems.Add(item);
            }
            var model = new TimeSuggestionViewModel { AllUsers = this.manager.db.Users.ToList(), SelectedUsers = null };
            model.AllMeetings = this.manager.db.Meetings.ToList();
            return View(model);
        }

        // Confirm the time suggestion for Get - Todo Need to finish
        [HttpPost]
        public ActionResult SendTimeSuggestion(TimeSuggestionViewModel model) {
            ApplicationUser Anv = this.manager.db.Users.Find(User.Identity.GetUserId());
            var user = this.manager.db.Users.Find(User.Identity.GetUserId());
            var timeSuggestion = new TimeSuggestion() { Sender = user };

            if (model.MeetingID != null){
                timeSuggestion.Meeting = this.manager.db.Meetings.Find(int.Parse(model.MeetingID));
                timeSuggestion.Sender = user;
                List<ApplicationUser> invi = new List<ApplicationUser>() { };
                timeSuggestion.Invited = this.manager.db.Users.Single(x => x.Id == model.SelectedUsers);
                this.manager.db.TimeSuggestions.Add(timeSuggestion);

                this.manager.db.SaveChanges();
                return RedirectToAction("AllTimeSuggestion");
            }
            else {
                ViewBag.Message = "Please choose a meeting";
                return RedirectToAction("SendTimeSuggestion");
            }
        }

        // Creates a Invitation list and presents it
        public ActionResult InvitationList(){
            var users = this.manager.db.Users.ToList();
            bool Check = false;
            var list = new List<InvitationViewModel>();
            foreach (var item in users) {

                var listobj = new InvitationViewModel { Name = item.Name, User = item, Checked = Check };
                list.Add(listobj);
            }
            return View(list);
        }

        // Brings all suggestions
        public ActionResult AllTimeSuggestion(){
            ViewBag.Me = User.Identity.GetUserId();
            var suggestionList = this.manager.db.TimeSuggestions.Include(x => x.Sender).Include(x => x.Invited).Include(x => x.Dates).Include(x => x.Meeting).ToList();
            return View(suggestionList);
        }

        // Saves the specific time suggestion (in to the database)
        [HttpPost]
        public ActionResult SaveTS(TimeSuggestion timeSuggestion) {
            if (timeSuggestion.Accepted)this.manager.db.TimeSuggestions.Single(x => x.ID == timeSuggestion.ID).Accepted = true;
            else this.manager.db.TimeSuggestions.Single(x => x.ID == timeSuggestion.ID).Denied = true;
            this.manager.db.SaveChanges();
            var suggestionList = this.manager.db.TimeSuggestions.Include(x => x.Sender).Include(x => x.Invited).Include(x => x.Dates).ToList();
            return RedirectToAction("AllTimeSuggestion");
        }


        public ActionResult MeetingInfo(int id){
            var info = manager.getInfo(id);
            return View(info);
        }
    }

    // Model for the invitation
    public class InvitationViewModel {
        public string Name { get; set; }
        public bool Checked { get; set; }
        public ApplicationUser User { get; set; }
    }
}