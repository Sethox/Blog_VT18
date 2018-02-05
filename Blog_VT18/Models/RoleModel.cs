using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_VT18.Models
{
    public class RoleModel
    {
        public string ID { set; get; }
        public string Name { set; get; }
        public string Role { set; get; }
        public ApplicationUser ByWhom { set; get; }
    }
}