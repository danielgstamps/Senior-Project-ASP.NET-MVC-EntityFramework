using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class AnswerViewModel
    {
        public int Id { get; set; }

        public int Answer { get; set; }
    }
}