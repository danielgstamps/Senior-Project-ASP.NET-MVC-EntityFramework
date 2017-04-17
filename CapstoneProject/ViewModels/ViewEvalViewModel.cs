using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class ViewEvalViewModel
    {
        public Evaluation Eval { get; set; }

        public List<Question> QuestionList { get; set; }

        public List<string> Answers { get; set; }
    }
}