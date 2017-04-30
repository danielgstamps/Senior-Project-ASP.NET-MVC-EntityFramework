using System;
using System.Linq;
using CapstoneProject.Models;

namespace CapstoneProject.HtmlExtensions
{
    public static class HtmlExtensions
    {
        public static bool CohortHasOpenEval(Cohort cohort, int typeId)
        {
            try
            {
                return cohort.Employees.Any(
                    emp => emp.Evaluations.Any(
                        eval => eval.TypeID == typeId &&
                        eval.OpenDate <= DateTime.Today &&
                        eval.CloseDate > DateTime.Today &&
                        !eval.IsComplete()));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CohortHasClosedEval(Cohort cohort, int typeId)
        {
            try
            {
                var firstEmployee = cohort.Employees.First();
                var allEvalsOfType = firstEmployee.Evaluations.Where(eval => eval.TypeID == typeId && !eval.IsComplete());
                return allEvalsOfType.First().CloseDate <= DateTime.Today;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CohortFinishedType(Cohort cohort, int typeId)
        {
            if (cohort.Employees.Count == 0)
            {
                return false;
            }

            return cohort.Employees.All(
                em => em.Evaluations.Count > 0 &&
                em.Evaluations.Count(ev => ev.TypeID == typeId) > 0 &&
                em.Evaluations.Where(ev => ev.TypeID == typeId).All(
                     e => e.IsComplete()));
        }

        public static string CohortEvalOpenDate(Cohort cohort, int typeId)
        {
            try
            {
                var firstEmployee = cohort.Employees.First();
                var allEvalsOfType = firstEmployee.Evaluations.Where(eval => eval.TypeID == typeId && !eval.IsComplete());
                return allEvalsOfType.First().OpenDate.ToString("d");
            }
            catch (Exception)
            {
                return "0/0/0000";
            }
        }

        public static string CohortEvalCloseDate(Cohort cohort, int typeId)
        {
            try
            {
                var firstEmployee = cohort.Employees.First();
                var allEvalsOfType = firstEmployee.Evaluations.Where(eval => eval.TypeID == typeId && !eval.IsComplete());
                return allEvalsOfType.First().CloseDate.ToString("d");
            }
            catch (Exception)
            {
                return "0/0/0000";
            }
        }

        public static bool CohortLocked(Cohort cohort)
        {
            return cohort.Type1Assigned || 
                   cohort.Type2Assigned ||
                   CohortHasOpenEval(cohort, 1) || 
                   CohortHasOpenEval(cohort, 2) ||
                   CohortFinishedStage(cohort, "Baseline", 1) ||
                   CohortFinishedStage(cohort, "Baseline", 2);
        }

        public static bool CohortCanEvaluate(Cohort cohort)
        {
            return cohort.Employees.Count != 0 &&
                   cohort.Employees.All(e => e.EmailConfirmed) &&
                   !(cohort.Type1Assigned && cohort.Type2Assigned) &&
                   !CohortFinished(cohort);
        }

        public static bool CohortFinished(Cohort cohort)
        {
            return CohortFinishedStage(cohort, "Summative", 1) &&
                   CohortFinishedStage(cohort, "Summative", 2);
        }

        public static bool CohortFinishedStage(Cohort cohort, string stageName, int typeId)
        {
            if (cohort.Employees.Count == 0)
            {
                return false;
            }

            try
            {
                foreach (var emp in cohort.Employees)
                {
                    var evalsOfType = emp.Evaluations.Where(eval => eval.TypeID.Equals(typeId));
                    if (!evalsOfType.Any())
                    {
                        return false;
                    }

                    var evalsOfTypeAndStage = evalsOfType.Where(eval => eval.Stage.StageName.Equals(stageName));
                    if (!evalsOfTypeAndStage.Any())
                    {
                        return false;
                    }

                    if (!evalsOfTypeAndStage.All(eval => eval.IsComplete()))
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}