using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Question
    {
        private const int MAX_LENGTH = 500;

        [Display(Name = "Question ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Question ID required")]
        public int QuestionID { get; set; }

        [Display(Name = "Question")]
        [StringLength(MAX_LENGTH)]
        [Required(ErrorMessage = "Must have question")]
        public string QuestionText { get; set; }

        public int CategoryID { get; set; }
        
        public virtual Category Category { get; set; }
    }
}