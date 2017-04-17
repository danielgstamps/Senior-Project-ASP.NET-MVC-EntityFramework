using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class AssignRatersViewModel
    {
        public int? EvalId { get; set; }

        public List<Rater> Raters { get; set; }

        // Doesn't work. Need to find a way to ensure emails aren't duplicates - but only for this eval.
        // Probably easiest to do in the controller.
        //IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        //{
        //    if (Raters.Select(s => s.Email).Distinct().Count() < Raters.Count)
        //    {
        //        yield return new ValidationResult("The Open Date must come before the Close Date");
        //    }
        //}
    }
}