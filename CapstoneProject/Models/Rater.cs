using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Rater
    {
        public int RaterID { get; set; }

        [Display(Name = "First TypeName")]
        public string FirstName { get; set; }

        [Display(Name = "Last TypeName")]
        public string LastName { get; set; }

        public string Role { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public List<int> Answers { get; set; }

        public int EvaluationID { get; set; }

        public virtual Evaluation Evaluation { get; set; }
    }
}