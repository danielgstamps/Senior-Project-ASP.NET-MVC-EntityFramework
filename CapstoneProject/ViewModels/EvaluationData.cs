using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.ViewModels
{
    public class EvaluationData
    {
        public string SelfAnswers { get; set; }
        public int NumberOfSupervisors { get; set; }
        public int NumberOfCoworkers { get; set; }
        public int NumberOfSupervisees { get; set; }
        public string[] SupervisorAnswers { get; set; }
        public string[] CoworkerAnswers { get; set; }
        public string[] SuperviseeAnswers { get; set; }
    }
}