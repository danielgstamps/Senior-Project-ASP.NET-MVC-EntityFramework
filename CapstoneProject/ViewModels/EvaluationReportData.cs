using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.ViewModels
{
    public class EvaluationReportData
    {
        public string SelfAnswers { get; set; }
        public Dictionary<Rater, string> RaterAnswers { get; set; }
    }
}