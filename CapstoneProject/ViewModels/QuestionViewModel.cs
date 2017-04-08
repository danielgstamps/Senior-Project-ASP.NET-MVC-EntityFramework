using System.Collections.Generic;
using System.Web.Mvc;

namespace CapstoneProject.ViewModels
{
    public class QuestionViewModel
    {
        public string QuestionText { get; set; }

        public int CategoryID { get; set; }

        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}