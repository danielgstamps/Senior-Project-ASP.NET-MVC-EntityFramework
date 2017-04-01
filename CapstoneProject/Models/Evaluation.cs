using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Evaluation
    { 
        private const int MAX_LENGTH = 500;

        [Display(Name = "Evaluation ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Evaluation ID required")]
        public int EvaluationID { get; set; }

        public int TypeID { get; set; }

        public virtual Type Type { get; set; }

        //[Display(Name = "Comment")]
        //[StringLength(MAX_LENGTH)]
        //public string Comment { get; set; }

        public int EmployeeID { get; set; }

        public virtual Employee Employee { get; set; }

        public int StageID { get; set; }

        public virtual Stage Stage { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime ClosedDate { get; set; }

        public List<int> SelfAnswers { get; set; }

        public virtual ICollection<Rater> Raters { get; set; }
    }
}