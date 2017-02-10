using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Evaluation
    {
        [Display(Name = "Evaluation ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Evaluation ID required")]
        public string EvaluationID { get; set; }

        public string CategoryID { get; set; }

        public Category Category { get; set; }
    }
}