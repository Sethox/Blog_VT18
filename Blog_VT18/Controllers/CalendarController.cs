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
            var List = new List<Meeting>();
            var UserList = new List<InvitedToMeetings>();
            var events = manager.getEventTimes(); if(manager.getEventTimes().Count > 0) {
                foreach(var i in events) {
                    List.Add(i);
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
    }
}


