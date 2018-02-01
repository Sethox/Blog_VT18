using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_VT18.Models
{
    public class TimeSuggestionViewModel
    {
        
        public int Id { get; set; }
        public List<ApplicationUser> AllUsers { get; set; }
        public List<ApplicationUser> SelectedUsers { get; set; }
        public List<Date> DateList { get; set; }
    }
}