using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Stage
    {
        private const int MAX_STAGE_NAME_LENGTH = 9;

        public int StageID { get; set; }

        [Display(Name = "Stage")]
        [StringLength(MAX_STAGE_NAME_LENGTH)]
        [Required(ErrorMessage = "Stage name required")]
        public string StageName { get; set; }

        public virtual ICollection<Evaluation> Evals { get; set; }
    }
}