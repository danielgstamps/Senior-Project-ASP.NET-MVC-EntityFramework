using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class EvaluationCreateViewModel : IValidatableObject
    {
        public int CohortID { get; set; }

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

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (CloseDate <= OpenDate)
            {
                yield return new ValidationResult("The Open Date must come before the Close Date");
            }

            //if (OpenDate < DateTime.Today)
            //{
            //    yield return new ValidationResult("The Open Date cannot be in the past.");
            //}
        }
    }
}