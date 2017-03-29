using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapstoneProject.Models
{
    public class QuestionViewModel
    {
        public string QuestionText { get; set; }

        public int CategoryID { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}