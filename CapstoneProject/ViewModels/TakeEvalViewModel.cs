using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class TakeEvalViewModel
    {
        public int EvalId { get; set; }

        public List<QuestionViewModel> AllQuestions { get; set; }

        public int TypeId { get; set; }

        public int? RaterId { get; set; }
    }
}