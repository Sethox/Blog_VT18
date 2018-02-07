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
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            return View(scheduler);
        }
        public ContentResult Data() {
            List<Meeting> events = manager.getEventTimes();
            List<Meeting> List = new List<Meeting>();

            if (manager.getEventTimes().Count > 0)
            {
                foreach (var item in events)
                {
                    List.Add(item);

                }
            }
            var data = new SchedulerAjaxData(List);
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
        public ActionResult SendTimeSuggestion() {

            
            TimeSuggestion Suggestion = new TimeSuggestion();
            ViewBag.Me = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            var list = new List<ApplicationUser> { user };
            Suggestion.Invited = user;
            var invitedList = user;

            //   Date ettDatum = new Date();
            //    ettDatum.TheDate = System.DateTime.Now;
            //    ettDatum.Id = 1;

            var listItems = new List<ApplicationUser> {
    };
            foreach (var item in db.Users)
            {
                listItems.Add(item);
            }
          // COMMENT - "selectedUsers" WAS = invitedList before merge
            var model = new TimeSuggestionViewModel { AllUsers = db.Users.ToList(), SelectedUsers = null };


            model.AllMeetings = db.Meetings.ToList();
            //  model.DateList.Add(ettDatum);
            return View(model);
        }

        [HttpPost]
        public ActionResult SendTimeSuggestion(TimeSuggestionViewModel model) {
            ApplicationUser Anv = db.Users.Find(User.Identity.GetUserId());
            
            var user = db.Users.Find(User.Identity.GetUserId());
            var timeSuggestion = new TimeSuggestion() { Sender = user };
            timeSuggestion.Meeting = db.Meetings.Find(int.Parse(model.MeetingID) );
            timeSuggestion.Sender = user;
            List<ApplicationUser> invi = new List<ApplicationUser>() { };
            //foreach (var item in model.SelectedUsers)
            //{
            //    invi.Add(db.Users.Single(x=> x.Id == item));
            //}
            timeSuggestion.Invited = db.Users.Single(x => x.Id == model.SelectedUsers);

            //foreach (var item in model.DateList.Where(x=> x.Date != null))
            //{           Date ettDatum = new Date();
            //            ettDatum.TheDate = item.Date;
            //            var list = new List<Date>() { ettDatum };
            //            timeSuggestion.Dates = list;
                 
            //}

            


            db.TimeSuggestions.Add(timeSuggestion);
            db.SaveChanges();






            return RedirectToAction("AllTimeSuggestion");  
            }


        public ActionResult AllTimeSuggestion() {
            
            ViewBag.Me = User.Identity.GetUserId();
            var suggestionList = db.TimeSuggestions.Include(x => x.Sender).Include(x=> x.Invited).Include(x=> x.Dates).Include(x=> x.Meeting).ToList();

            return View(suggestionList);
        }
        [HttpPost]
        public ActionResult SaveTS(TimeSuggestion timeSuggestion)
        {
            

            db.TimeSuggestions.Single(x => x.ID == timeSuggestion.ID).Accepted = true;
            db.SaveChanges();

            var suggestionList = db.TimeSuggestions.Include(x => x.Sender).Include(x => x.Invited).Include(x => x.Dates).ToList();
            return RedirectToAction("AllTimeSuggestion");
        }


    }
}
