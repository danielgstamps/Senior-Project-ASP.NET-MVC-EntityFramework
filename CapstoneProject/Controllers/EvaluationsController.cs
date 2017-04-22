using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace CapstoneProject.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
       // private readonly ApplicationDbContext userDB = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

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

        public IUnitOfWork UnitOfWork { get; set; } = new UnitOfWork();

        [Authorize(Roles = "Admin")]
        public ActionResult AdminEvalsIndex()
        {
            var evalsOrderedByEmployeeID =
                this.UnitOfWork.EvaluationRepository.Get().OrderBy(e => e.Employee.EmployeeID).ToList();
            return View("AdminEvalsIndex", evalsOrderedByEmployeeID);
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
                        Text = question.QuestionText,
                        Id = count
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

                return eval.Raters.Count > 0 ? 
                    RedirectToAction("AssignRaters", new {id = eval.EvaluationID}) : 
                    RedirectToAction("EmployeeEvalsIndex", new {id = eval.EmployeeID});
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
            return RedirectToAction("RaterCleanup", "Raters", new {id = rater.RaterID});
        }

        // GET AssignRaters
        public ActionResult AssignRaters(int? id)
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
            if (employee == null || !employee.Email.Equals(User.Identity.GetUserName()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var model = new AssignRatersViewModel
            {
                EvalId = eval.EvaluationID,
                Raters = eval.Raters.ToList()
            };

            // If the employee has a previously completed eval (with raters), pull as much rater info from it as possible.
            if (employee.Evaluations.Any(e => e.IsComplete() && e.Raters.Count != 0))
            {
                var completedEval = employee.Evaluations.First(e => e.IsComplete() && e.Raters.Count != 0);
                var previousRaters = completedEval.Raters.ToList();
                foreach (var modelRater in model.Raters)
                {
                    foreach (var prevRater in previousRaters)
                    {
                        if (modelRater.Role.Equals(prevRater.Role) && // Roles are the same
                            !model.Raters.Exists(r => r.Email.Equals(prevRater.Email))) // model didn't already use this rater.
                        {
                            modelRater.Name = prevRater.Name;
                            modelRater.Email = prevRater.Email;
                        }
                    }
                }
            }
            // Otherwise, just initialize the rater fields as empty.
            else
            {
                foreach (var rater in model.Raters)
                {
                    rater.Name = "";
                    rater.Email = "";
                }
            }         

            return View("AssignRaters", model);
        }

        // POST AssignRaters
        [HttpPost]
        public ActionResult AssignRaters(AssignRatersViewModel model)
        {
            if (model == null || model.Raters.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(model.EvalId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            if (eval.Raters.Count != model.Raters.Count)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            // Check for duplicate emails (Not using Unique tags because they don't need to be unique in the db).
            if (model.Raters.DistinctBy(r => r.Email).Count() != model.Raters.Count)
            {
                TempData["DuplicateError"] = "Please enter a unique email address for each rater.";
                return RedirectToAction("AssignRaters", new { id = model.EvalId });
            }

            var i = 0;
            foreach (var rater in eval.Raters)
            {
                rater.Name = model.Raters[i].Name;
                rater.Email = model.Raters[i].Email;

                //if (ModelState.IsValid)
                //{
                //    MailMessage mail = new MailMessage();
                //    mail.To.Add(rater.Email);
                //    mail.From = new MailAddress("admin@gmail.com");
                //    mail.Subject = "Evaluation";
                //    var Body = "";
                //    mail.Body = Body;
                //    mail.IsBodyHtml = true;
                //    var smtp = new SmtpClient
                //    {
                //        Host = "smtp.gmail.com",
                //        Port = 587,
                //        UseDefaultCredentials = false,
                //        Credentials = new NetworkCredential
                //            ("admin@gmail.com", "123123"),
                //        EnableSsl = true
                //    };
                //    // Enter seders User name and password
                //    smtp.Send(mail);
                //}

                UnitOfWork.Save();
                i++;
            }

            TempData["TakeEvalSuccess"] = "Successfully completed evaluation.";
            return RedirectToAction("EmployeeEvalsIndex", new { id = eval.EmployeeID });
        }

        // GET: EditRaters
        public ActionResult EditRaters(int? id) //evalID
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
            if (employee == null || !employee.Email.Equals(User.Identity.GetUserName()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var model = new AssignRatersViewModel
            {
                EvalId = eval.EvaluationID,
                Raters = eval.Raters.ToList()
            };

            return View("EditRaters", model);
        }

        // POST: EditRaters
        [HttpPost]
        public ActionResult EditRaters(AssignRatersViewModel model)
        {
            if (model == null || model.Raters.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.Raters.DistinctBy(r => r.Email).Count() != model.Raters.Count)
            {
                TempData["DuplicateError"] = "Please enter a unique email address for each rater.";
                return RedirectToAction("EditRaters", new { id = model.EvalId });
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(model.EvalId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var i = 0;
            foreach (var rater in eval.Raters)
            {
                rater.Name = model.Raters[i].Name;
                rater.Email = model.Raters[i].Email;
                UnitOfWork.Save();
                i++;
            }

            TempData["EditRaterSuccess"] = "Successfully updated raters.";
            return RedirectToAction("EmployeeEvalsIndex", new { id = eval.EmployeeID });
        }

        // GET: ReplaceRater
        public ActionResult ReplaceRater(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var raterToReplace = UnitOfWork.RaterRepository.GetByID(id);
            if (raterToReplace == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(raterToReplace.EvaluationID);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = UnitOfWork.EmployeeRepository.GetByID(eval.EmployeeID);
            if (employee == null || !employee.Email.Equals(User.Identity.GetUserName()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            var model = new ReplaceRaterViewModel()
            {
                EvalId = eval.EvaluationID,
                RaterToReplace = raterToReplace,
                NewRater = new Rater { Role = raterToReplace.Role }
            };

            return View("ReplaceRater", model);
        }

        // POST: ReplaceRater
        [HttpPost]
        public ActionResult ReplaceRater(ReplaceRaterViewModel model)
        {
            if (model == null ||
                model.EvalId == null ||
                model.RaterToReplace == null ||
                model.NewRater == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(model.EvalId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            if (eval.Raters.Any(r => r.Email.Equals(model.NewRater.Email)))
            {
                TempData["DuplicateError"] = "This evaluation already has a rater with that email.";
                return RedirectToAction("ReplaceRater", new { id = model.RaterToReplace.RaterID });
            }

            var raterToDisable = UnitOfWork.RaterRepository.GetByID(model.RaterToReplace.RaterID);
            if (raterToDisable == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            raterToDisable.Disabled = true;
            UnitOfWork.Save();

            eval.Raters.Add(model.NewRater);
            UnitOfWork.Save();

            TempData["ReplaceRaterSuccess"] = "Successfully replaced rater.";
            return RedirectToAction("EditRaters", new { id = eval.EvaluationID });
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
                return HttpNotFound();
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

            var answersList = ConvertAnswersToList(evaluation.SelfAnswers);
            var model = new ViewEvalViewModel()
            {
                Eval = evaluation,
                QuestionList = questionList,
                Answers = answersList
            };

            return View("Details", model);
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
            if (cohort.Type1Assigned)
            {
                itemList.RemoveAt(0);
            }
            if (cohort.Type2Assigned)
            {
                itemList.RemoveAt(1);
            }
            model.TypeList = itemList;

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
            if (selectedStageName.Equals("Formative") && !cohort.IsStageComplete("Baseline", model.TypeID))
            {
                TempData["StageError"] = "Formative can only be selected after Baseline is completed.";
                return RedirectToAction("Create", new { cohortId = (int)TempData["CohortID"] });
            }
            if (selectedStageName.Equals("Summative") && !cohort.IsStageComplete("Formative", model.TypeID))
            {
                TempData["StageError"] = "Summative can only be selected after Formative is completed.";
                return RedirectToAction("Create", new { cohortId = (int)TempData["CohortID"] });
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
                return HttpNotFound();
            }

            TempData["CohortID"] = cohortId;
            TempData["TypeId"] = typeId;
            TempData["TypeDisplay"] = typeId;
            TempData["CohortName"] = cohort.Name;

            EvaluationCreateViewModel model = new EvaluationCreateViewModel();

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
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Get the first eval that isn't complete, of this type.
            var eval = employee.Evaluations.First(e => !e.IsComplete() && e.TypeID == typeId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            model.CohortID = (int)cohortId;
            model.TypeID = typeId.Value;

            model.StageID = eval.StageID;
            model.OpenDate = eval.OpenDate;
            model.CloseDate = eval.CloseDate;

            model.NumberOfSupervisors = NumberOfRatersWithRole(eval, "Supervisor");
            model.NumberOfCoworkers = NumberOfRatersWithRole(eval, "Coworker");
            model.NumberOfSupervisees = NumberOfRatersWithRole(eval, "Supervisee"); ;

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
            if (selectedStageName.Equals("Formative") && !cohort.IsStageComplete("Baseline", model.TypeID))
            {
                TempData["StageError"] = "Formative can only be selected after Baseline is completed.";
                return RedirectToAction("Edit", new { cohortId = (int)TempData["CohortID"], typeId = (int)TempData["TypeId"] });
            }
            if (selectedStageName.Equals("Summative") && !cohort.IsStageComplete("Formative", model.TypeID))
            {
                TempData["StageError"] = "Summative can only be selected after Formative is completed.";
                return RedirectToAction("Edit", new { cohortId = (int)TempData["CohortID"], typeId = (int)TempData["TypeId"] });
            }

            // Remove target evals
            foreach (var emp in cohort.Employees)
            {
                // If this throws an exception, that means an employee has more than 1 eval of the same type. Which it shouldn't. That'd be bad.
                var eval = emp.Evaluations.Single(e => !e.IsComplete() && e.TypeID == model.TypeID);
                UnitOfWork.EvaluationRepository.Delete(eval.EvaluationID);
                UnitOfWork.Save();
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

            var employeeEvals = employee.Evaluations;
            return View(employeeEvals);
        }

        private int NumberOfRatersWithRole(Evaluation eval, string role)
        {
            return eval.Raters.Count(r => r.Role.Equals(role));
        }

        private string ConvertAnswersToString(List<QuestionViewModel> questions)
        {
            var answerString = "";
            foreach (var question in questions)
            {
                answerString += question.SelectedAnswer.ToString();
            }
            return answerString;
        }

        private List<string> ConvertAnswersToList(string answers)
        {
            var list = new List<string>();
            for (var i = 0; i < answers.Length; i++)
            {
                if (answers[i].Equals('0') && answers[i - 1].Equals('1'))
                {
                    list.Add("10");
                    continue;
                }
                list.Add(answers[i].ToString());
            }
            return list;
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
