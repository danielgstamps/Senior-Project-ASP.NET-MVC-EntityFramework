using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.ViewModels
{
    public class TakeEvalViewModel
    {
        public List<QuestionViewModel> AllQuestions { get; set; }

        public int TypeId { get; set; }
    }
}