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

    public class SchedulerHub : Hub {
        public void Send(string update) {
            this.Clients.All.addMessage(update);
        }
       }
    }


namespace Blog_VT18.Controllers {
    public class CalendarController : BaseController {
        public RepositoryManager manager { get; set; }
        public CalendarController() { manager = new RepositoryManager(); }

        public ActionResult Index() {
            //Being initialized in that way, scheduler will use CalendarController.Data as a the datasource and CalendarController.Save to process changes
            var scheduler = new DHXScheduler(this);
            /*
             * It's possible to use different actions of the current controller
             *      var scheduler = new DHXScheduler(this);     
             *      scheduler.DataAction = "ActionName1";
             *      scheduler.SaveAction = "ActionName2";
             * Or to specify full paths
             *      var scheduler = new DHXScheduler();
             *      scheduler.DataAction = Url.Action("Data", "Calendar");
             *      scheduler.SaveAction = Url.Action("Save", "Calendar");
             *
             * The default codebase folder is ~/Scripts/dhtmlxScheduler. It can be overriden:
             *      scheduler.Codebase = Url.Content("~/customCodebaseFolder");
             */
            scheduler.InitialDate = new DateTime();
            scheduler.Extensions.Add(SchedulerExtensions.Extension.LiveUpdates);
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            return View(scheduler);
        }
        public ContentResult Data() {
            List<Meeting> calendar = manager.GetMeetings();
            List<Meeting> List = new List<Meeting>();
            List<InvitedToMeetings> UserList = new List<InvitedToMeetings>();

            foreach (var item in calendar)
            {
                string fyllMig = "";

                var nyList = db.TimeSuggestions.Where(x => x.Meeting.ID == item.ID & x.Accepted == false & x.Denied == false).Select(x => x.Invited.Name).ToList();
                var acceptedList = db.TimeSuggestions.Where(x => x.Meeting.ID == item.ID & x.Accepted==true).Select(x => x.Invited.Name).ToList();
                var deniedList = db.TimeSuggestions.Where(x => x.Meeting.ID == item.ID & x.Denied == true).Select(x => x.Invited.Name).ToList();



                var listan = manager.getInvited(item.ID);
                if (nyList.Count() <= 0) {
                    nyList.Add("ingen");
                }
                foreach (var spot in nyList)
                {
                    fyllMig = fyllMig + "\n" + spot;
                }
                fyllMig = fyllMig + "\n \n Accepted";
                foreach (var spot in acceptedList)
                {
                    fyllMig = fyllMig + "\n" + spot;
                }
                fyllMig = fyllMig + "\n \n Denied";
                foreach (var spot in deniedList)
                {
                    fyllMig = fyllMig + "\n" + spot;
                }

                var aEvent = new Meeting
                {
                    ID = item.ID,
                    text = item.text + " \nBooked by: " + item.Booker.Name + "\nWaiting for response: " + fyllMig,
                    start_date = item.start_date,
                    end_date = item.end_date
                };
                List.Add(aEvent);

            }
            var data = new SchedulerAjaxData(
                List
                );
            return (ContentResult)data;
        }
        public ContentResult Save(int? id, FormCollection actionValues) {
            var calendar = manager.getEventTimes();
            var action = new DataAction(actionValues);
            try {
                var changedEvent = (Meeting)DHXEventsHelper.Bind(typeof(Meeting), actionValues);
                switch(action.Type) {
                    case DataActionTypes.Insert:
                        //do insert
                        //action.TargetId = changedEvent.id;
                        //assign postoperational id
                        // Todo: Get invited list/people to add a meeting
                        /*changedEvent.Text += changedEvent.Text + " \nBooked by: " + item.Booker.Name + "\nInvited: " + listan;*/
                        manager.setEventTime(changedEvent);
                        break;
                    case DataActionTypes.Delete:
                        //do delete
                        break;
                    default:// "update"                          
                        //do update
                        break;
                }
            } catch {
                action.Type = DataActionTypes.Error;
            }
            return (ContentResult)new AjaxSaveResponse(action);
        }

        public ActionResult SendTimeSuggestion()
        {
            TimeSuggestion Suggestion = new TimeSuggestion();
            ViewBag.Me = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            var list = new List<ApplicationUser> { user };
            Suggestion.Invited = user;
            var invitedList = user;


            var listItems = new List<ApplicationUser>
            {
            };
            foreach (var item in db.Users)
            {
                listItems.Add(item);
            }
            var model = new TimeSuggestionViewModel { AllUsers = db.Users.ToList(), SelectedUsers = null };


            model.AllMeetings = db.Meetings.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult SendTimeSuggestion(TimeSuggestionViewModel model)
        {
            ApplicationUser Anv = db.Users.Find(User.Identity.GetUserId());

            var user = db.Users.Find(User.Identity.GetUserId());
            var timeSuggestion = new TimeSuggestion() { Sender = user };
            timeSuggestion.Meeting = db.Meetings.Find(int.Parse(model.MeetingID));
            timeSuggestion.Sender = user;
            List<ApplicationUser> invi = new List<ApplicationUser>() { };
            timeSuggestion.Invited = db.Users.Single(x => x.Id == model.SelectedUsers);
            db.TimeSuggestions.Add(timeSuggestion);
            db.SaveChanges();
            return RedirectToAction("AllTimeSuggestion");
        }




        public ActionResult InvitationList()
        {
            var users = db.Users.ToList();
            bool Check = false;
            var list = new List<InvitationViewModel>();

            foreach (var item in users)
            {
                var listobj = new InvitationViewModel { Name = item.Name, User = item, Checked = Check };

                list.Add(listobj);
            }
            return View(list);

        }









        //var timeSuggestion = new TimeSuggestion();

        //var senderId = User.Identity.GetUserId();
        //var sender = db.Users.Find(senderId);

        //timeSuggestion.Sender = sender;
        //timeSuggestion.Invited = model.SelectedUsers;
        //timeSuggestion.Dates = model.DateList;

        //db.TimeSuggestions.Add(timeSuggestion);
        //db.SaveChanges();


        // return View();




        public ActionResult AllTimeSuggestion()
        {

            ViewBag.Me = User.Identity.GetUserId();
            var suggestionList = db.TimeSuggestions.Include(x => x.Sender).Include(x => x.Invited).Include(x => x.Dates).Include(x => x.Meeting).ToList();

            return View(suggestionList);

        }
        [HttpPost]
        public ActionResult SaveTS(TimeSuggestion timeSuggestion)
        {

            if (timeSuggestion.Accepted)
            {
            db.TimeSuggestions.Single(x => x.ID == timeSuggestion.ID).Accepted = true;

            }
            else
            {
                db.TimeSuggestions.Single(x => x.ID == timeSuggestion.ID).Denied = true;
            }
            
            db.SaveChanges();

            var suggestionList = db.TimeSuggestions.Include(x => x.Sender).Include(x => x.Invited).Include(x => x.Dates).ToList();
            return RedirectToAction("AllTimeSuggestion");
        }
    }





public class InvitationViewModel
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
        public ApplicationUser User { get; set; } 
    }
    
}
