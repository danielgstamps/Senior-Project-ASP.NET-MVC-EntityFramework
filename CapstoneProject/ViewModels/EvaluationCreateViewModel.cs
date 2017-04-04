using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class EvaluationCreateViewModel
    {
        public Cohort CohortToEvaluate { get; set; }

        [Display(Name = "Type")]
        public int TypeID { get; set; }

        [Display(Name = "Stage")]
        public int StageID { get; set; }

        public IEnumerable<SelectListItem> TypeList { get; set; }

        public IEnumerable<SelectListItem> StageList { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Open Date")]
        public DateTime OpenDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Close Date")]
        public DateTime CloseDate { get; set; }

        public bool[] RaterOptions { get; set; }
    }
}