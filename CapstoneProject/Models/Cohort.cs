using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CapstoneProject.Models
{
    public class Cohort
    {
        private const int MAX_NAME_LENGTH = 50;

        [Display(Name = "Cohort ID")]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be a non-negative whole number")]
        [Required(ErrorMessage = "Cohort ID required")]
        public int CohortID { get; set; }

        [Display(Name = "Cohort Name")]
        [StringLength(MAX_NAME_LENGTH)]
        [Required(ErrorMessage = "Cohort name required")]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public bool Type1Assigned { get; set; }

        public bool Type2Assigned { get; set; }

        public bool HasOpenEval(int typeId)
        {
            try
            {
                var firstEmployee = Employees.First();
                var allEvalsOfType = firstEmployee.Evaluations.Where(eval => eval.TypeID == typeId);
                return allEvalsOfType.First().OpenDate <= DateTime.Today.Date;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string EvalOpenDate(int typeId)
        {
            try
            {
                var firstEmployee = Employees.First();
                var allEvalsOfType = firstEmployee.Evaluations.Where(eval => eval.TypeID == typeId);
                return allEvalsOfType.First().OpenDate.GetValueOrDefault().ToString("d");
            }
            catch (Exception)
            {
                return "0/0/0000";
            }
        }

        public bool IsStageComplete(string stageName, int typeId)
        {
            try
            {
                foreach (var emp in Employees)
                {
                    var evalsOfType = emp.Evaluations.Where(eval => eval.TypeID.Equals(typeId));
                    var evalsOfTypeAndStage = evalsOfType.Where(eval => eval.Stage.StageName.Equals(stageName));
                    if (!evalsOfTypeAndStage.Any())
                    {
                        return false;
                    }

                    if (evalsOfTypeAndStage.All(eval => eval.IsComplete()))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}