using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog_VT18.Models
{
    public class TimeSuggestionViewModel
    {
        public int Id { get; set; }
        public List<ApplicationUser> AllUsers { get; set; }
        public List<ApplicationUser> SelectedUsers { get; set; }
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public List<DateTime> DateList { get; set;}
        public virtual Meeting Meeting { get; set; }
    }
}