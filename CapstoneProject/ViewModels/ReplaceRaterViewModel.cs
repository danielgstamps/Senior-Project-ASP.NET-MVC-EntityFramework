using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CapstoneProject.Models;

namespace CapstoneProject.ViewModels
{
    public class ReplaceRaterViewModel
    {
        public int? EvalId { get; set; }

        public Rater RaterToReplace { get; set; }

        public Rater NewRater { get; set; }
    }
}