using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Cohort
    {
        private const int MAX_NAME_LENGTH = 50;

        [Display(Name = "Cohort ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Cohort ID required")]
        public int CohortID { get; set; }

        [Display(Name = "Name")]
        [StringLength(MAX_NAME_LENGTH)]
        [Required(ErrorMessage = "Cohort name required")]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}