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
             * 
             * Or to specify full paths
             *      var scheduler = new DHXScheduler();
             *      scheduler.DataAction = Url.Action("Data", "Calendar");
             *      scheduler.SaveAction = Url.Action("Save", "Calendar");
             */

            /*
             * The default codebase folder is ~/Scripts/dhtmlxScheduler. It can be overriden:
             *      scheduler.Codebase = Url.Content("~/customCodebaseFolder");
             */
            scheduler.InitialDate = new DateTime();
            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;
            return View(scheduler);
        }

        //public ContentResult Data2() {
        //    var data = new SchedulerAjaxData(
        //            new List<CalendarEvent>{
        //                new CalendarEvent{
        //                    id = 1,
        //                    text = "Sample Event",
        //                    start_date = new DateTime(2018, 01, 27, 6, 00, 00),
        //                    end_date = new DateTime(2018, 01, 27, 8, 00, 00)
        //                },
        //                new CalendarEvent{
        //                    id = 2,
        //                    text = "New Event",
        //                    start_date = new DateTime(2018, 01, 26, 9, 00, 00),
        //                    end_date = new DateTime(2018, 01, 26, 12, 00, 00)
        //                },
        //                new CalendarEvent{
        //                    id = 3,
        //                    text = "Multiday Event",
        //                    start_date = new DateTime(2018, 01, 25, 10, 00, 00),
        //                    end_date = new DateTime(2018, 01, 30, 12, 00, 00)
        //                }
        //            }
        //        );
        //    return (ContentResult)data;
        //}
        public ContentResult Data() {
            List<Meeting> calendar = manager.GetMeetings();
            List<CalendarEvent> List = new List<CalendarEvent>();
            foreach(var item in calendar) {
                var aEvent = new CalendarEvent {
                    id = item.ID,
                    text = item.Info + " Booked by: " + item.Booker + " Invited: " + item.Invited,
                    start_date = item.DateFrom,
                    end_date = item.DateTo

                };
                List.Add(aEvent);

            }
            var data = new SchedulerAjaxData(
                List
                );
            return (ContentResult)data;
        }

        public ContentResult Save(int? id, FormCollection actionValues) {
            string action_type = actionValues["!nativeeditor_status"];
            Int64 source_id = Int64.Parse(actionValues["id"]);
            Int64 target_id = source_id;

            var action = new DataAction(actionValues);
            try {
                var changedEvent = (CalendarEvent)DHXEventsHelper.Bind(typeof(CalendarEvent), actionValues);
                switch(action.Type) {
                    case DataActionTypes.Insert:
                        //do insert


                        //action.TargetId = changedEvent.id;//assign postoperational id
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

