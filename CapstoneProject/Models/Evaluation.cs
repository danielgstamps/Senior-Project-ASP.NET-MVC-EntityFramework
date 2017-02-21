using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Evaluation
    {
        private const int MAX_STAGE_NAME_LENGTH = 9;
        private const int TYPE_ONE = 1;
        private const int TYPE_TWO = 2;

        [Display(Name = "Evaluation ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Evaluation ID required")]
        public int EvaluationID { get; set; }

        [Display(Name = "Stage")]
        [StringLength(MAX_STAGE_NAME_LENGTH)]
        [Required(ErrorMessage = "Stage name required")]
        public string Stage { get; set; }

        [Display(Name = "Type")]
        [Range(TYPE_ONE, TYPE_TWO)]
        [Required(ErrorMessage = "Type required. May only be Type 1 or Type 2")]
        public int Type { get; set; }
        
        public int EmployeeID { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}