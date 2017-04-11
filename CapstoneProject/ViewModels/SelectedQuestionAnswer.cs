using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class SelectedQuestionAnswer
    {
        public Question Question { get; set; }

        [Required]
        public string SelectedAnswer { get; set; }
    }
}