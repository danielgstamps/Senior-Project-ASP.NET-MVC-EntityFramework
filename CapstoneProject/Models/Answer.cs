using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }

        [Display(Name = "Strongly Disagree")]
        public string StronglyDisagree { get; set; }

        [Display(Name = "Disagree")]
        public string Disagree { get; set; }

        [Display(Name = "Neutral")]
        public string Neutral { get; set; }

        [Display(Name = "Agree")]
        public string Agree { get; set; }

        [Display(Name = "Strongly Agree")]
        public string StronglyAgree { get; set; }

        public bool StronglyDisagreeSelected { get; set; }

        public bool DisagreeSelected { get; set; }

        public bool NeutralSelected { get; set; }

        public bool AgreeSelected { get; set; }

        public bool StronglyAgreeSelected { get; set; }
    }
}