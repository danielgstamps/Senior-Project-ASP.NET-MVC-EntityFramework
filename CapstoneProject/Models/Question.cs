using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public sealed class Question
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

        public string SelectedAnswer { get; set; }

        //public ICollection<Category> Categories { get; set; }
        public Category Category { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public Question()
        {
            Answers = new List<Answer>();
        }
    }
}