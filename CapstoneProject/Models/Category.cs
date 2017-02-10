using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Category
    {
        private const int MAX_NAME_LENGTH = 50;

        [Display(Name = "Category ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Category ID required")]
        public int CategoryID { get; set; }

        [Display(Name = "Category Name")]
        [StringLength(MAX_NAME_LENGTH)]
        [Required(ErrorMessage = "Category name required")]
        public string Name { get; set; }
    }
}