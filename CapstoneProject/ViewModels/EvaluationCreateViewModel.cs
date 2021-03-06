﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OpenDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Close Date")]
        public DateTime? CloseDate { get; set; }

        [Range(0, 1, ErrorMessage="Please enter 0 or 1.")] // please don't actually try 100. My poor wallet.
        public int NumberOfSupervisors { get; set; }

        [Range(0, 10, ErrorMessage = "Please enter a number between 0 and 10.")]
        public int NumberOfCoworkers { get; set; }

        [Range(0, 10, ErrorMessage = "Please enter a number between 0 and 10.")]
        public int NumberOfSupervisees { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (CloseDate <= OpenDate)
            {
                yield return new ValidationResult("The Open Date must come before the Close Date");
            }

            if (OpenDate < DateTime.Today)
            {
                yield return new ValidationResult("The Open Date cannot be in the past.");
            }
        }
    }
}