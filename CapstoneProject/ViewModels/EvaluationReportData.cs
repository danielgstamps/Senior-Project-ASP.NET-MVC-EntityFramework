using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.ViewModels
{
    public class EvaluationReportData
    {
        public int EvaluationID { get; set; }
        public string EmployeeName { get; set; }
        public string TypeName { get; set; }
        public string StageName { get; set; }
        public string CurrentDate { get { return DateTime.Today.ToString().Split(' ')[0]; } }
        public List<Rater> Raters { get; set; }
        public List<Category> Categories { get; set; }
        public List<int> EmployeeAnswers { get; set; }
        public List<int> SupervisorAvgAnswers { get; set; }
        public List<int> CoworkerAvgAnswers { get; set; }
        public List<int> SuperviseeAvgAnswers { get; set; }
    }
}