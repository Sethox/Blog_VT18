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

                var listan = manager.getInvited(item.ID);
                if (listan=="") {
                    listan = "None invited";
                }
                var aEvent = new Meeting
                {
                    ID = item.ID,
                    text = item.text + " \nBooked by: " + item.Booker.Name + "\nInvited: " + listan,
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
        public ActionResult SendTimeSuggestion() {
            TimeSuggestion Suggestion = new TimeSuggestion();

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

          //  model.DateList.Add(ettDatum);
            return View(model);
        }

        [HttpPost]
        public ActionResult SendTimeSuggestion(TimeSuggestionViewModel model) {
            ApplicationUser Anv = db.Users.Find(User.Identity.GetUserId());

           //List<ApplicationUser> aa = new List<ApplicationUser>();
           // aa.Add(Anv);
         
            //model.SelectedUsers = aa;
             var user = db.Users.Find(User.Identity.GetUserId());
            var timeSuggestion = new TimeSuggestion() { Sender = user };

            timeSuggestion.Sender = user;
            List<ApplicationUser> invi = new List<ApplicationUser>() { };
            //foreach (var item in model.SelectedUsers)
            //{
            //    invi.Add(db.Users.Single(x=> x.Id == item));
            //}
            timeSuggestion.Invited = db.Users.Single(x => x.Id == model.SelectedUsers);

            foreach (var item in model.DateList.Where(x=> x.Date != null))
            {           Date ettDatum = new Date();
                        ettDatum.TheDate = item.Date;
                        var list = new List<Date>() { ettDatum };
                        timeSuggestion.Dates = list;
                 
            }

            

  // COMMENT - THIS WAS MASTER'S before merge
       //   List<ApplicationUser> aa = new List<ApplicationUser>();
       //   aa.Add(Anv);
            // var aaa = aa.ToList();
       //   model.SelectedUsers = aa;
       //   var user = db.Users.Find(User.Identity.GetUserId());
       //   var timeSuggestion = new TimeSuggestion() { Invited = model.SelectedUsers, Sender = user };

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
        


        public ActionResult AllTimeSuggestion() {

            ViewBag.Me = User.Identity.GetUserId();
            var suggestionList = db.TimeSuggestions.Include(x => x.Sender).Include(x=> x.Invited).Include(x=> x.Dates).ToList();

            return View(suggestionList);

        }   
        }  

    public class InvitationViewModel
    {
        public string Name { get; set; }
        public bool Checked { get; set; }
        public ApplicationUser User { get; set; } 
    }



        }
    }

}
