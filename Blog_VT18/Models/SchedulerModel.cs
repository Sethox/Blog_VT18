using System;

namespace Blog_VT18.Models
{
    public class CalendarEvent
    {
        public CalendarEvent() { }
        public CalendarEvent(Calender CP)
        {
            this.id = CP.ID;
            this.text = CP.text;
            this.start_date = CP.start_date;
            this.end_date = CP.end_date;
        }

        //id, text, start_date and end_date properties are mandatory
        public int id { get; set; }
        public string text { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
    }
}