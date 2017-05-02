using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;
using Castle.Core.Internal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MvcRazorToPdf;

namespace CapstoneProject.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private ApplicationUserManager _userManager;
        private IUnitOfWork _unitOfWork = new UnitOfWork();

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ApplicationSignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
        }

        public EvaluationsController()
        {
            
        }

        public EvaluationsController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
            set
            {
                _unitOfWork = value;
            }
        }

        // GET: Evaluations/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? cohortId)
        {
            if (cohortId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(cohortId);
            if (cohort == null)
            {
                return HttpNotFound();
            }

            // Link manipulation could crash the page without this.
            if (HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Summative", 1) && HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Summative", 2))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TempData["CohortID"] = cohortId;
            TempData["CohortName"] = cohort.Name;

            var model = new EvaluationCreateViewModel
            {
                CohortID = (int)cohortId,
                NumberOfSupervisors = 1,
                NumberOfCoworkers = 2,
                NumberOfSupervisees = 2,
                OpenDate = DateTime.Today.Date,
                CloseDate = DateTime.Today.AddDays(1).Date,

                TypeList = UnitOfWork.TypeRepository.dbSet.Select(t => new SelectListItem()
                {
                    Value = t.TypeID.ToString(),
                    Text = t.TypeName,
                }),

                StageList = UnitOfWork.StageRepository.dbSet.Select(t => new SelectListItem()
                {
                    Value = t.StageID.ToString(),
                    Text = t.StageName
                })
            };

            // Remove types if the cohort already has them assigned.
            var itemList = model.TypeList.ToList();
            if (cohort.Type1Assigned || HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Summative", 1))
            {
                itemList.RemoveAt(0);
            }
            if (cohort.Type2Assigned || HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Summative", 2))
            {
                itemList.RemoveAt(1);
            }
            model.TypeList = itemList;

            ViewBag.BaselineId = UnitOfWork.StageRepository.Get(s => s.StageName.Equals("Baseline")).First().StageID;
            return View("Create", model);
        }

        // POST: Evaluations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvaluationCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["DateError"] = "Open Date cannot be in the past, and must come before Close Date.";
                return RedirectToAction("Create", new { cohortId = (int)TempData["CohortID"] });
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(model.CohortID);

            // Stage order enforcement
            var selectedStageName = UnitOfWork.StageRepository.GetByID(model.StageID).StageName;
            if (selectedStageName.Equals("Formative") && !HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Baseline", model.TypeID))
            {
                TempData["StageError"] = "Formative can only be selected after Baseline is completed.";
                return RedirectToAction("Create", new { cohortId = (int)TempData["CohortID"] });
            }
            if (selectedStageName.Equals("Summative") && !HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Formative", model.TypeID))
            {
                TempData["StageError"] = "Summative can only be selected after Formative is completed.";
                return RedirectToAction("Create", new { cohortId = (int)TempData["CohortID"] });
            }

            // Disallow selecting stages that are already complete.
            if (HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, selectedStageName, model.TypeID))
            {
                TempData["StageError"] = "This cohort has already completed the " + selectedStageName +
                    " stage for Type " + model.TypeID.ToString() + ".";
                return RedirectToAction("Create", new { cohortId = (int)TempData["CohortID"] });
            }

            // If stage != baseline, pull rater numbers from baseline eval
            if (selectedStageName != "Baseline")
            {
                var prevEval = UnitOfWork.EvaluationRepository.Get().First(e =>
                    e.Employee.CohortID == cohort.CohortID &&
                    e.IsComplete() &&
                    e.Stage.StageName.Equals("Baseline") &&
                    e.TypeID == model.TypeID);

                model.NumberOfSupervisors = NumberOfRatersWithRole(prevEval, "Supervisor");
                model.NumberOfCoworkers = NumberOfRatersWithRole(prevEval, "Coworker");
                model.NumberOfSupervisees = NumberOfRatersWithRole(prevEval, "Supervisee");
            }

            foreach (var emp in cohort.Employees)
            {
                var eval = new Evaluation
                {
                    Employee = emp,
                    Type = UnitOfWork.TypeRepository.GetByID(model.TypeID),
                    Stage = UnitOfWork.StageRepository.GetByID(model.StageID),
                    OpenDate = model.OpenDate.Value,
                    CloseDate = model.CloseDate.Value,
                    CompletedDate = null,
                    SelfAnswers = "",
                    Raters = GenerateRaterList(model.NumberOfSupervisors, model.NumberOfCoworkers, model.NumberOfSupervisees)
                };

                UnitOfWork.EvaluationRepository.Insert(eval);
                UnitOfWork.Save();
            }

            if (model.TypeID == 1)
            {
                cohort.Type1Assigned = true;
                UnitOfWork.Save();
            }
            if (model.TypeID == 2)
            {
                cohort.Type2Assigned = true;
                UnitOfWork.Save();
            }

            TempData["CreateSuccess"] = "Successfully created evaluation for " + cohort.Name + ".";
            return RedirectToAction("Index", "Cohorts");
        }

        // GET: Evaluations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var evaluation = UnitOfWork.EvaluationRepository.GetByID(id);
            if (evaluation == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = UnitOfWork.EmployeeRepository.GetByID(evaluation.EmployeeID);
            if (employee == null || !employee.Email.Equals(User.Identity.GetUserName()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var questionList = new List<Question>();
            var categories = evaluation.Type.Categories;
            foreach (var cat in categories)
            {
                foreach (var question in cat.Questions)
                {
                    questionList.Add(question);
                }
            }

            var answersList = evaluation.SelfAnswers.Split(',').ToList();
            var model = new ViewEvalViewModel
            {
                Eval = evaluation,
                QuestionList = questionList,
                Answers = answersList
            };

            return View("Details", model);
        }

        // GET: Evaluations/Edit?cohortId=x&typeId=1,2
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? cohortId, int? typeId)
        {
            if (cohortId == null || typeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(cohortId);
            var type = UnitOfWork.TypeRepository.GetByID(typeId);
            if (cohort == null || type == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TempData["CohortID"] = cohortId;
            TempData["TypeId"] = typeId;
            TempData["TypeDisplay"] = typeId;
            TempData["CohortName"] = cohort.Name;

            var model = new EvaluationCreateViewModel();

            // Get all Types.
            model.TypeList = UnitOfWork.TypeRepository.dbSet.Select(t => new SelectListItem()
            {
                Value = t.TypeID.ToString(),
                Text = t.TypeName,
            });

            // Get all Stages.
            model.StageList = UnitOfWork.StageRepository.dbSet.Select(t => new SelectListItem()
            {
                Value = t.StageID.ToString(),
                Text = t.StageName,
            });

            // Get the first employee in the cohort with at least 1 eval.
            var employee = cohort.Employees.First(e => e.Evaluations.Count != 0);
            if (employee == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the first eval that isn't complete, of this type.
            var eval = employee.Evaluations.First(e => !e.IsComplete() && e.TypeID == typeId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.CohortID = (int)cohortId;
            model.TypeID = typeId.Value;

            model.StageID = eval.StageID;
            model.OpenDate = eval.OpenDate;
            model.CloseDate = eval.CloseDate;

            model.NumberOfSupervisors = NumberOfRatersWithRole(eval, "Supervisor");
            model.NumberOfCoworkers = NumberOfRatersWithRole(eval, "Coworker");
            model.NumberOfSupervisees = NumberOfRatersWithRole(eval, "Supervisee"); ;

            ViewBag.BaselineId = UnitOfWork.StageRepository.Get(s => s.StageName.Equals("Baseline")).First().StageID;
            return View("Edit", model);
        }

        // POST: Evaluations/Edit?cohortId=x&typeId=1,2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvaluationCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["DateError"] = "Open Date cannot be in the past, and must come before Close Date.";
                return RedirectToAction("Edit", new { cohortId = (int)TempData["CohortID"], typeId = (int)TempData["TypeId"] });
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(model.CohortID);
            if (cohort == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Stage order enforcement
            var selectedStageName = UnitOfWork.StageRepository.GetByID(model.StageID).StageName;
            if (selectedStageName.Equals("Formative") && !HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Baseline", model.TypeID))
            {
                TempData["StageError"] = "Formative can only be selected after Baseline is completed.";
                return RedirectToAction("Edit", new { cohortId = (int)TempData["CohortID"], typeId = (int)TempData["TypeId"] });
            }
            if (selectedStageName.Equals("Summative") && !HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Formative", model.TypeID))
            {
                TempData["StageError"] = "Summative can only be selected after Formative is completed.";
                return RedirectToAction("Edit", new { cohortId = (int)TempData["CohortID"], typeId = (int)TempData["TypeId"] });
            }

            // Disallow selecting stages that are already complete.
            if (HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, selectedStageName, model.TypeID))
            {
                TempData["StageError"] = "This cohort has already completed the " + selectedStageName +
                    " stage for Type " + model.TypeID.ToString() + ".";
                return RedirectToAction("Edit", new { cohortId = (int)TempData["CohortID"], typeId = (int)TempData["TypeId"] });
            }

            // Remove target evals
            foreach (var emp in cohort.Employees)
            {
                // If this throws an exception, that means an employee has more than 1 incomplete eval of the same type. Which should be impossible.
                var eval = emp.Evaluations.Single(e => !e.IsComplete() && e.TypeID == model.TypeID);
                UnitOfWork.EvaluationRepository.Delete(eval.EvaluationID);
                UnitOfWork.Save();
            }

            // If stage != baseline, pull rater numbers from baseline eval
            if (selectedStageName != "Baseline")
            {
                var prevEval = UnitOfWork.EvaluationRepository.Get().First(e =>
                    e.Employee.CohortID == cohort.CohortID &&
                    e.IsComplete() &&
                    e.Stage.StageName.Equals("Baseline") &&
                    e.TypeID == model.TypeID);

                model.NumberOfSupervisors = NumberOfRatersWithRole(prevEval, "Supervisor");
                model.NumberOfCoworkers = NumberOfRatersWithRole(prevEval, "Coworker");
                model.NumberOfSupervisees = NumberOfRatersWithRole(prevEval, "Supervisee");
            }

            // Recreate evals (I remove/recreate so the emails re-send, and the Rater logic is cleaner).
            foreach (var emp in cohort.Employees)
            {
                var eval = new Evaluation
                {
                    Employee = emp,
                    Type = UnitOfWork.TypeRepository.GetByID(model.TypeID),
                    Stage = UnitOfWork.StageRepository.GetByID(model.StageID),
                    OpenDate = model.OpenDate.Value, // This "PossibleInvalidOperation" will never happen. It'd break way up there^ if the dates were null.
                    CloseDate = model.CloseDate.Value,
                    SelfAnswers = "",
                    Raters = GenerateRaterList(model.NumberOfSupervisors, model.NumberOfCoworkers, model.NumberOfSupervisees)
                };

                UnitOfWork.EvaluationRepository.Insert(eval);
                UnitOfWork.Save();
            }

            TempData["EditSuccess"] = "Successfully updated evaluation.";
            return RedirectToAction("Index", "Cohorts");
        }

        // GET: Evaluations/Delete/
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? cohortId, int? typeId)
        {
            if (cohortId == null || typeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var emp = UnitOfWork.EmployeeRepository.Get().First(e => e.CohortID == cohortId);
            var eval = emp.Evaluations.Single(e => e.TypeID == typeId && !e.IsComplete());
            return View("Delete", eval);
        }

        // POST: Evaluations/Delete/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int cohortId, int typeId)
        {
            var cohort = UnitOfWork.CohortRepository.GetByID(cohortId);
            if (cohort == null || cohort.Employees.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employees = cohort.Employees;
            foreach (var emp in employees)
            {
                UnitOfWork.EvaluationRepository.Delete(emp.Evaluations.Single(e => e.TypeID == typeId && !e.IsComplete()));
                UnitOfWork.Save();
            }

            switch (typeId)
            {
                case 1:
                    cohort.Type1Assigned = false;
                    UnitOfWork.Save();
                    break;
                case 2:
                    cohort.Type2Assigned = false;
                    UnitOfWork.Save();
                    break;
            }

            TempData["DeleteSuccess"] = "Successfully deleted evaluation.";
            return RedirectToAction("Index", "Cohorts");
        }

        [Authorize]
        public ActionResult EmployeeEvalsIndex(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = UnitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null || !employee.Email.Equals(User.Identity.GetUserName()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var employeeEvals = employee.Evaluations.OrderBy(e => e.TypeID);
            return View(employeeEvals);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminEvalsIndex()
        {
            var evalsOrderedByEmployeeId =
                UnitOfWork.EvaluationRepository.Get().OrderBy(e => e.Employee.EmployeeID).ToList();
            return View("AdminEvalsIndex", evalsOrderedByEmployeeId);
        }

        [AllowAnonymous]
        //GET: Evaluations/TakeEvaluation/5
        public ActionResult TakeEvaluation(int? id, int? raterId, string code)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(id);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = UnitOfWork.EmployeeRepository.GetByID(eval.EmployeeID);

            // If raterId is null, this is an employee taking their eval. Make sure the logged-in user is correct and hasn't already finished this eval.
            if (raterId == null)
            {
                if (employee == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (!employee.Email.Equals(User.Identity.GetUserName()))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                if (!string.IsNullOrEmpty(eval.SelfAnswers))
                {
                    TempData["EvalAlreadyComplete"] = "You have already completed this evaluation.";
                    return RedirectToAction("EmployeeEvalsIndex", new { id = employee.EmployeeID });
                }
                if (eval.OpenDate > DateTime.Today.Date)
                {
                    TempData["EvalNotYetOpen"] = "This evaluation is not open yet.";
                    return RedirectToAction("EmployeeEvalsIndex", new { id = employee.EmployeeID });
                }
                if (eval.CloseDate <= DateTime.Today.Date)
                {
                    TempData["EvalClosed"] = "This evaluation is closed.";
                    return RedirectToAction("EmployeeEvalsIndex", new { id = employee.EmployeeID });
                }
            }

            var model = new TakeEvalViewModel
            {
                AllQuestions = new List<QuestionViewModel>(),
                TypeId = eval.TypeID,
                EvalId = eval.EvaluationID,
                RaterId = raterId
            };

            var count = 1;
            foreach (var category in eval.Type.Categories)
            {
                foreach (var question in category.Questions)
                {
                    model.AllQuestions.Add(new QuestionViewModel
                    {
                        Id = count,
                        Text = question.QuestionText,
                        Category = question.Category.Name
                    });
                    count++;
                }
            }

            if (raterId == null)
            {
                ViewBag.TakeEvalHeader = "Self-Evaluation for " + employee.FirstName + " " + employee.LastName + ".";
            }
            else
            {
                var rater = UnitOfWork.RaterRepository.GetByID(raterId);
                if (rater == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ViewBag.TakeEvalHeader = "Evaluating " + eval.Employee.FirstName + " " + eval.Employee.LastName +
                                         " as a " + rater.Role + ".";
                ViewBag.TakeEvalSubHeader = "Logged in as " + rater.Name + ".";
            }

            return View("TakeEvaluation", model);
        }

        // POST: TakeEval
        [HttpPost]
        public ActionResult TakeEvaluation(TakeEvalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.AllQuestions == null || model.AllQuestions.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(model.EvalId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.RaterId == null) //Employee is taking the evaluation.
            {
                eval.SelfAnswers = ConvertAnswersToString(model.AllQuestions);
                eval.CompletedDate = DateTime.Now;
                UnitOfWork.Save();
                if (eval.Raters.Count > 0)
                {
                    if (eval.Stage.StageName.Equals("Baseline"))
                    {
                        return RedirectToAction("AssignRaters", "Raters", new { id = eval.EvaluationID });
                    }
                    return RedirectToAction("ConfirmRaters", "Raters", new { id = eval.EvaluationID });
                }

                CheckCohortAndResetFlags(eval.Employee.CohortID);
                return RedirectToAction("EmployeeEvalsIndex", new {id = eval.EmployeeID});
            }

            // Rater is taking the evaluation
            var rater = UnitOfWork.RaterRepository.GetByID(model.RaterId);
            if (rater == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!rater.Email.Equals(User.Identity.GetUserName()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            rater.Answers = ConvertAnswersToString(model.AllQuestions);
            UnitOfWork.Save();
            CheckCohortAndResetFlags(eval.Employee.CohortID);
            return RedirectToAction("RaterCleanup", "Raters", new {id = rater.RaterID});
        }

        /// <summary>
        /// Shows an evaluation report as an HTML page.
        /// </summary>
        /// <param name="id">ID of the desired evaluation</param>
        /// <returns>A ViewResult that renders the report</returns>
        public ActionResult ShowReportAsHtml(int? id)
        {
            var eval = UnitOfWork.EvaluationRepository.GetByID(id);
            var reportData = CreateReportData(eval);
            return View("ReportAsHtml", reportData);
        }

        public ActionResult ShowReportAsPdf(int? id)
        {
            var eval = UnitOfWork.EvaluationRepository.GetByID(id);
            var reportData = CreateReportData(eval);
            return new PdfActionResult("ReportAsPdf", reportData);
        }

        private EvaluationReportData CreateReportData(Evaluation eval)
        {
            var supervisors = new List<Rater>();
            var coworkers = new List<Rater>();
            var supervisees = new List<Rater>();
            //GroupRatersByRole(eval, supervisors, coworkers, supervisees);
            //var ratersToShow = new List<Rater>();
            //ratersToShow.AddRange(supervisors);
            //ratersToShow.AddRange(coworkers);
            //ratersToShow.AddRange(supervisees);
            //var numOfQuestions = GetNumberOfQuestions(eval);

            groupRatersByRole(eval, supervisors, coworkers, supervisees);
            var ratersToShow = addRatersToShow(supervisors, coworkers, supervisees);
            var numOfQuestions = getNumberOfQuestions(eval);
            var employeeAnswers = getEmployeeAnswers(eval);
            var supervisorAvgs = getQuestionAvgPerRole(supervisors, numOfQuestions);
            var coworkerAvgs = getQuestionAvgPerRole(coworkers, numOfQuestions);
            var superviseeAvgs = getQuestionAvgPerRole(supervisees, numOfQuestions);

            //var allAvgAnswers = new List<double>();
            //var allAnswers = new List<List<int>>();
            //allAnswers.Add(employeeAnswers);
            //if (!supervisorAvgs.IsNullOrEmpty())
            //{
            //    allAnswers.Add(supervisorAvgs);
            //}
            //if (!coworkerAvgs.IsNullOrEmpty())
            //{
            //    allAnswers.Add(coworkerAvgs);
            //}
            //if (!superviseeAvgs.IsNullOrEmpty())
            //{
            //    allAnswers.Add(superviseeAvgs);
            //}
            //getAvgForAllResponders(numOfQuestions, allAnswers, allAvgAnswers);
            var allAnswers = compileAllAnswers(employeeAnswers, supervisorAvgs, coworkerAvgs, superviseeAvgs);
            var allAvgAnswers = getAvgForAllResponders(numOfQuestions, allAnswers);

            return new EvaluationReportData
            {
                EvaluationID = eval.EvaluationID,
                EmployeeName = eval.Employee.FirstName + " " + eval.Employee.LastName,
                TypeName = eval.Type.TypeName,
                StageName = eval.Stage.StageName,
                Raters = ratersToShow,
                Categories = eval.Type.Categories.ToList(),
                EmployeeAnswers = employeeAnswers,
                SupervisorAvgAnswers = supervisorAvgs,
                CoworkerAvgAnswers = coworkerAvgs,
                SuperviseeAvgAnswers = superviseeAvgs,
                AllAvgAnswers = allAvgAnswers
            };
        }

        private static List<double> getAvgForAllResponders(int numOfQuestions, List<List<int>> allAnswers)
        {
            var allAvgAnswers = new List<double>();
            for (int i = 0; i < numOfQuestions; i++)
            {
                var total = 0.0;
                var avg = total / allAnswers.Count;
                allAvgAnswers.Add(avg);
            }
            return allAvgAnswers;
        }

        private static List<Rater> addRatersToShow(List<Rater> supervisors, List<Rater> coworkers, List<Rater> supervisees)
        {
            var ratersToShow = new List<Rater>();
            ratersToShow.AddRange(supervisors);
            ratersToShow.AddRange(coworkers);
            ratersToShow.AddRange(supervisees);
            return ratersToShow;
        }

        private static List<List<int>> compileAllAnswers(List<int> employeeAnswers, List<int> supervisorAvgs, List<int> coworkerAvgs, List<int> superviseeAvgs)
        {
            var allAnswers = new List<List<int>>();
            allAnswers.Add(employeeAnswers);
            if (!supervisorAvgs.IsNullOrEmpty())
            {
                allAnswers.Add(supervisorAvgs);
            }
            if (!coworkerAvgs.IsNullOrEmpty())
            {
                allAnswers.Add(coworkerAvgs);
            }
            if (!superviseeAvgs.IsNullOrEmpty())
            {
                allAnswers.Add(superviseeAvgs);
            }
            return allAnswers;
        }

        private void groupRatersByRole(Evaluation eval, List<Rater> supervisors, List<Rater> coworkers, List<Rater> supervisees)
        {
            foreach (var rater in eval.Raters)
            {
                PutRaterInGroup(supervisors, coworkers, supervisees, rater);
            }
        }

        private void PutRaterInGroup(List<Rater> supervisors, List<Rater> coworkers, List<Rater> supervisees, Rater rater)
        {
            switch (rater.Role)
            {
                case "Supervisor":
                    supervisors.Add(rater);
                    break;
                case "Coworker":
                    coworkers.Add(rater);
                    break;
                case "Supervisee":
                    supervisees.Add(rater);
                    break;
            }
        }

        private int getNumberOfQuestions(Evaluation eval)
        {
            var numOfQuestions = 0;
            foreach (var category in eval.Type.Categories)
            {
                foreach (var question in category.Questions)
                {
                    numOfQuestions++;
                }
            }

            return numOfQuestions;
        }

        private List<int> getEmployeeAnswers(Evaluation eval)
        {
            var employeeAnswers = new List<int>();
            foreach (var answerString in eval.SelfAnswers.Split(',').ToList())
            {
                employeeAnswers.Add(Convert.ToInt32(answerString));
            }
            return employeeAnswers;
        }

        private List<int> getQuestionAvgPerRole(List<Rater> raters, int numOfQuestions)
        {
            if (raters.IsNullOrEmpty())
            {
                return new List<int>();
            }
            var avgs = new List<int>();
            for (var index = 0; index < numOfQuestions; index++)
            {
                calculateAverage(raters, avgs, index);
            }
            return avgs;
        }

        private void calculateAverage(List<Rater> raters, List<int> avgs, int index)
        {
            var totalForQuestion = 0;
            var avgForQuestion = 0;
            foreach (var rater in raters)
            {
                var answer = Convert.ToInt32(rater.Answers.Split(',')[index]);
                totalForQuestion += answer;
            }
            avgForQuestion = totalForQuestion / raters.Count;
            avgs.Add(avgForQuestion);
        }

        private int NumberOfRatersWithRole(Evaluation eval, string role)
        {
            return eval.Raters.Count(r => r.Role.Equals(role) && !r.Disabled);
        }

        private string ConvertAnswersToString(List<QuestionViewModel> questions)
        {
            var answerString = "";
            foreach (var question in questions)
            {
                answerString += question.SelectedAnswer + ",";
            }
            answerString = answerString.TrimEnd(',');
            return answerString;
        }

        private List<Rater> GenerateRaterList(int numSupervisors, int numCoworkers, int numSupervisees)
        {
            var raters = new List<Rater>();
            for (var i = 0; i < numSupervisors; i++)
            {
                raters.Add(new Rater()
                {
                    Role = "Supervisor",
                    Name = "Name",
                    Email = "email@address.com"
                });
            }

            for (var i = 0; i < numCoworkers; i++)
            {
                raters.Add(new Rater()
                {
                    Role = "Coworker",
                    Name = "Name",
                    Email = "email@address.com"
                });
            }

            for (var i = 0; i < numSupervisees; i++)
            {
                raters.Add(new Rater()
                {
                    Role = "Supervisee",
                    Name = "Name",
                    Email = "email@address.com"
                });
            }

            return raters;
        }

        private void CheckCohortAndResetFlags(int? cohortId)
        {
            if (cohortId == null)
            {
                return;
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(cohortId);
            if (cohort == null)
            {
                return;
            }

            if (HtmlExtensions.HtmlExtensions.CohortFinishedType(cohort, 1) && !HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Summative", 1))
            {
                cohort.Type1Assigned = false;
            }

            if (HtmlExtensions.HtmlExtensions.CohortFinishedType(cohort, 2) && !HtmlExtensions.HtmlExtensions.CohortFinishedStage(cohort, "Summative", 2))
            {
                cohort.Type2Assigned = false;
            }

            UnitOfWork.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
