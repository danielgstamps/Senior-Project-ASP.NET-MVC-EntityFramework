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
// ReSharper disable InconsistentNaming

namespace CapstoneProject.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private readonly ApplicationDbContext userDB = new ApplicationDbContext();
        private readonly ApplicationUserManager _userManager;

        public EvaluationsController()
        {
            
        }

        public EvaluationsController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public IUnitOfWork UnitOfWork { get; set; } = new UnitOfWork();

        //GET: Evaluations/TakeEvaluation/5
        public ActionResult TakeEvaluation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(id);
            if (eval == null)
            {
                return HttpNotFound();
            }

            // Probably need different authentication for Raters
            //var employee = UnitOfWork.EmployeeRepository.GetByID(eval.EmployeeID);
            //if (employee == null || !employee.Email.Equals(User.Identity.GetUserName()))
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            //}

            var model = new TakeEvalViewModel
            {
                AllQuestions = new List<QuestionViewModel>(),
                TypeId = eval.TypeID,
                EvalId = eval.EvaluationID
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

            eval.SelfAnswers = ConvertAnswersToString(model.AllQuestions);
            eval.CompletedDate = DateTime.Now;
            UnitOfWork.Save();

            if (eval.Raters.Count > 1)
            {
                return RedirectToAction("AssignRaters", new { id = eval.EvaluationID });
            }

            return RedirectToAction("EmployeeEvalsIndex", new { id = eval.EmployeeID });
        }

        // GET
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

            foreach (var rater in model.Raters)
            {
                rater.FirstName = "";
                rater.LastName = "";
                rater.Email = "";
            }

            return View("AssignRaters", model);
        }

        // POST
        [HttpPost]
        public async Task<ActionResult> AssignRaters(AssignRatersViewModel model)
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
                rater.FirstName = model.Raters[i].FirstName;
                rater.LastName = model.Raters[i].LastName;
                rater.Email = model.Raters[i].Email;

                if (ModelState.IsValid)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(rater.Email);
                    mail.From = new MailAddress("admin@gmail.com");
                    mail.Subject = "Evaluation";
                    var Body = "";
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential
                            ("admin@gmail.com", "123123"),
                        EnableSsl = true
                    };
                    // Enter seders User name and password
                    smtp.Send(mail);
                }

                UnitOfWork.Save();
                i++;
            }

            TempData["TakeEvalSuccess"] = "Successfully completed evaluation.";
            return RedirectToAction("EmployeeEvalsIndex", new { id = eval.EmployeeID });
        }

        // GET: EditRaters
        public ActionResult EditRaters(int? id)
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
        public async Task<ActionResult> EditRaters(AssignRatersViewModel model)
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
                rater.FirstName = model.Raters[i].FirstName;
                rater.LastName = model.Raters[i].LastName;
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
                TypeList = UnitOfWork.TypeRepository.dbSet.Select(t => new SelectListItem()
                {
                    Value = t.TypeID.ToString(),
                    Text = t.TypeName,
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

            // Get all stages.
            model.StageList = UnitOfWork.StageRepository.dbSet.Select(t => new SelectListItem()
            {
                Value = t.StageID.ToString(),
                Text = t.StageName
            });

            model.NumberOfSupervisors = 0;
            model.NumberOfCoworkers = 0;
            model.NumberOfSupervisees = 0;

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
                if (model.OpenDate != null)
                {
                    if (model.CloseDate != null)
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
                        SendEvaluationEmail(emp.EmployeeID, eval); // Commenting this out for now, it rustles Microsoft's jimmies.
                    }
                }
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
                if (model.OpenDate != null)
                {
                    if (model.CloseDate != null)
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
                    }
                }
                UnitOfWork.Save();
                // SendEvaluationEmail(emp.EmployeeID, eval); // Don't await this. Commenting this out for now, it rustles Microsoft's jimmies.
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
                    FirstName = "First Name",
                    LastName = "Last Name",
                    Email = "email@address.com"
                });
            }

            for (var i = 0; i < numCoworkers; i++)
            {
                raters.Add(new Rater()
                {
                    Role = "Coworker",
                    FirstName = "First Name",
                    LastName = "Last Name",
                    Email = "email@address.com"
                });
            }

            for (var i = 0; i < numSupervisees; i++)
            {
                raters.Add(new Rater()
                {
                    Role = "Supervisee",
                    FirstName = "First Name",
                    LastName = "Last Name",
                    Email = "email@address.com"
                });
            }

            return raters;
        }

        public ApplicationUserManager UserManager => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        private async void SendEvaluationEmail(int employeeID, Evaluation evaluation)
        {
            var employee = UnitOfWork.EmployeeRepository.GetByID(employeeID);
            var userAccount = userDB.Users.ToList().Find(u => u.Email == employee.Email);
            var userEmail = userAccount.Email;

            // TODO Specify EvaluationsController Action in first string param
            var callbackUrl = Url.Action("TakeEvaluation", "Evaluations", new { id = evaluation.EvaluationID }, protocol: Request.Url.Scheme);

            var emailSubject = "New Evaluation Available";
            var emailBody =
            "You have a new evaluation to complete. Here are the details: " +
            "\r\n\r\n" +
            "Type: " + evaluation.Type.TypeName +
            "\r\n\r\n" +
            "Stage: " + evaluation.Stage.StageName +
            "\r\n\r\n" +
            "Open Date: " + evaluation.OpenDate +
            "\r\n\r\n" +
            "Close Date: " + evaluation.CloseDate +
            "\r\n\r\n" +
            "Click <a href=\"" + callbackUrl + "\">here</a> to complete your evaluation.";

            await UserManager.SendEmailAsync(userAccount.Id, emailSubject, emailBody);
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
