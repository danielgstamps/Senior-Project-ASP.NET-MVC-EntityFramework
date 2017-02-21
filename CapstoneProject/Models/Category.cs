using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject.Models
{
    public class Category
    {
        private const int MAX_NAME_LENGTH = 50;
        private const int MAX_DESC_LENGTH = 500;

        [Display(Name = "Category ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Category ID required")]
        public int CategoryID { get; set; }

        [Display(Name = "Category Name")]
        [StringLength(MAX_NAME_LENGTH)]
        [Required(ErrorMessage = "Category name required")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(MAX_DESC_LENGTH)]
        [Required(ErrorMessage = "Description required. Cannot exede 500 characters")]
        public string Description { get; set; }

        public int EvaluationID { get; set; }

        public virtual Evaluation Evaluation { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}