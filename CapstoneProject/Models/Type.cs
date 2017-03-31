using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Type
    {
        public int TypeID { get; set; }

        [Display(Name = "Type")]
        public string TypeName { get; set; }
    }
}