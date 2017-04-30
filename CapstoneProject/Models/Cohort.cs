using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CapstoneProject.Models
{
    public class Cohort
    {
        private const int MaxNameLength = 50;

        [Display(Name = "Cohort ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Cohort ID required")]
        public int CohortID { get; set; }

        [Display(Name = "Cohort Name")]
        [StringLength(MaxNameLength)]
        [Required(ErrorMessage = "Cohort name required")]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public bool Type1Assigned { get; set; }

        public bool Type2Assigned { get; set; }
    }
}