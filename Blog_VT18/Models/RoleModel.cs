﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog_VT18.Models
{
    public class RoleModel
    {
        public string ID { set; get; }
        public string Name { set; get; }
        public string Role { set; get; }
        public List<SelectListItem> Roles { set; get; }
        public ApplicationUser ByWhom { set; get; }
        [DisplayName("Active")]
        public bool IsEnabled { set; get; }
    }
}