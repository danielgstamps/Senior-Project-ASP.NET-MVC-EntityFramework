using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public string QuestionText { get; set; }

        [Required]
        public int? Answer { get; set; }
    }
}