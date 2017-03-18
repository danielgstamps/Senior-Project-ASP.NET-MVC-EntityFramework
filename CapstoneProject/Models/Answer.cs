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

        public string AnswerText { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}