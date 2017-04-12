using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Rater
    {
        public int RaterID { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Role { get; set; }

        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Answers { get; set; }

        public int EvaluationID { get; set; }

        public virtual Evaluation Evaluation { get; set; }
    }
}