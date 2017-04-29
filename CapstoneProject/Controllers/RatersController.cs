using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CapstoneProject.Controllers
{
    public class RatersController : Controller
    {
        private ApplicationUserManager _userManager;
                                         
        public IUnitOfWork UnitOfWork { get; set; } = new UnitOfWork();

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        // GET AssignRaters
        public ActionResult AssignRaters(int? id) //evalId
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
                rater.Name = "";
                rater.Email = "";
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

            if (model.Raters.Any(r => r.Email.Equals(eval.Employee.Email)))
            {
                TempData["DuplicateError"] = "Nice try. You can't rate yourself.";
                return RedirectToAction("AssignRaters", new { id = eval.EvaluationID });
            }

            var i = 0;
            foreach (var rater in eval.Raters)
            {
                rater.Name = model.Raters[i].Name;
                rater.Email = model.Raters[i].Email;
                UnitOfWork.Save();
                i++;
            }

            return RedirectToAction("NotifyRatersNow", "Raters", new { id = eval.EvaluationID });
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

            var eval = UnitOfWork.EvaluationRepository.GetByID(model.EvalId);
            if (eval == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.Raters.DistinctBy(r => r.Email).Count() != model.Raters.Count)
            {
                TempData["DuplicateError"] = "Please enter a unique email address for each rater.";
                return RedirectToAction("EditRaters", new { id = model.EvalId });
            }

            if (model.Raters.Any(r => r.Email.Equals(eval.Employee.Email)))
            {
                TempData["DuplicateError"] = "Nice try. You can't rate yourself.";
                return RedirectToAction("EditRaters", new { id = model.EvalId });
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
            return RedirectToAction("EmployeeEvalsIndex", "Evaluations", new { id = eval.EmployeeID });
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

            if (model.NewRater.Email.Equals(eval.Employee.Email))
            {
                TempData["DuplicateError"] = "Nice try. You can't rate yourself.";
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
            return RedirectToAction("EditRaters", "Raters", new { id = eval.EvaluationID });
        }

        // GET RaterPrompt
        public async Task<ActionResult> RaterPrompt(int? id, int? raterId, string code)
        {
            if (id == null || raterId == 0 || string.IsNullOrEmpty(code))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var rater = UnitOfWork.RaterRepository.GetByID(raterId);
            var eval = UnitOfWork.EvaluationRepository.GetByID(id);
            if (rater == null || eval == null)
            {
                TempData["RaterError"] = "This evaluation is no longer available.";
                return View("ThankYou");
            }

            if (eval.CloseDate <= DateTime.Today || rater.Disabled)
            {
                TempData["RaterError"] = "This evaluation is no longer available.";
                return View("ThankYou");
            }

            if (!string.IsNullOrEmpty(rater.Answers))
            {
                TempData["RaterError"] = "You have already completed this evaluation.";
                return View("ThankYou");
            }

            var raterUser = UserManager.FindByName(rater.Email);
            if (raterUser == null)
            {
                TempData["RaterError"] = "This evaluation is no longer available.";
                return View("ThankYou");
            }

            var codeIsValid = UserManager.VerifyUserTokenAsync(raterUser.Id, "RaterLogin", code);

            if (codeIsValid.Result)
            {
                await SignInManager.SignInAsync(raterUser, false, false);
            }
            else
            {
                TempData["RaterError"] = "This evaluation is no longer available.";
                return View("ThankYou");
            }

            var model = new RaterPromptViewModel
            {
                EvalId = id.Value,
                RaterId = raterId.Value,
                Code = code
            };

            ViewBag.RaterName = rater.Name;
            return View("RaterPrompt", model);
        }

        // RaterCleanup
        public ActionResult RaterCleanup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var rater = UnitOfWork.RaterRepository.GetByID(id);
            if (rater == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
 
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.ExternalCookie);
            Session.Abandon();

            var raterUserAccount = UserManager.FindByEmail(rater.Email);
            if (raterUserAccount != null)
            {
                UserManager.Delete(raterUserAccount);
            }

            return View("ThankYou");
        }

        public ActionResult ConfirmRaters(int? id) //evalId
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
            if (employee == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            // If the employee has a previously completed eval (with raters), pull as much rater info from it as possible.
            if (employee.Evaluations.Any(e => e.IsComplete() && e.Raters.Count != 0))
            {
                var completedEval = employee.Evaluations.Last(e => e.IsComplete() && e.Raters.Count != 0);
                var previousRaters = completedEval.Raters.ToList();
                foreach (var rater in eval.Raters)
                {
                    foreach (var prevRater in previousRaters)
                    {
                        if (rater.Role.Equals(prevRater.Role) &&                    // Roles are the same
                            !prevRater.Disabled &&                                  // Previous rater isn't disabled.
                            !eval.Raters.Any(r => r.Email.Equals(prevRater.Email))) // model didn't already use this rater.
                        {
                            rater.Name = prevRater.Name;
                            rater.Email = prevRater.Email;
                            UnitOfWork.Save();
                        }
                    }
                }
            }

            var model = new AssignRatersViewModel()
            {
                EvalId = eval.EvaluationID,
                Raters = eval.Raters.ToList()
            };

            return View("ConfirmRaters", model);
        }

        [HttpPost]
        public ActionResult ConfirmRaters(AssignRatersViewModel model)
        {
            return RedirectToAction("NotifyRatersNow", new {id = model.EvalId });
        }

        // Send Notification Emails
        public ActionResult NotifyRater(int? raterId)
        {
            if (raterId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var rater = UnitOfWork.RaterRepository.GetByID(raterId);
            var evalId = rater.Evaluation.EvaluationID;
            SendRaterEmail(raterId.Value, evalId);

            TempData["EmailSuccess"] = "Sent notification to " + rater.Email;
            return RedirectToAction("EditRaters", "Raters", new { id = evalId });
        }

        // GET: NotifyRatersNow
        public ActionResult NotifyRatersNow(int? id) //evalId
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

            TempData["TakeEvalSuccess"] = "Evaluation complete. Please remember to send email notifications to your raters."; 
            return View("NotifyRatersNow", eval);
        }
        
        // POST: NotifyRatersNow
        [HttpPost]
        public ActionResult NotifyRatersNow(Evaluation model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eval = UnitOfWork.EvaluationRepository.GetByID(model.EvaluationID);
            foreach (var rater in eval.Raters)
            {
                if (!rater.Disabled && string.IsNullOrEmpty(rater.Answers))
                {
                    SendRaterEmail(rater.RaterID, model.EvaluationID);
                }
            }

            TempData["TakeEvalSuccess"] = "Evaluation complete. Email notifications sent to raters.";
            return RedirectToAction("EmployeeEvalsIndex", "Evaluations", new {id = model.EmployeeID});
        }

        private void SendRaterEmail(int raterId, int evalId)
        {
            var rater = UnitOfWork.RaterRepository.GetByID(raterId);
            var userAccount = GenerateTemporaryRaterAccount(raterId);
            var evaluation = UnitOfWork.EvaluationRepository.GetByID(evalId);
            var empFirstName = rater.Evaluation.Employee.FirstName;
            var empLastName = rater.Evaluation.Employee.LastName;
            var code = UserManager.GenerateUserToken("RaterLogin", userAccount.Id);

            var callbackUrl = Url.Action("RaterPrompt", "Raters", new { id = evaluation.EvaluationID, raterId, code }, Request.Url.Scheme);
            var emailSubject = "Evaluate Your Fellow Employee";

            var emailBody =
            "Hello " + rater.Name + "! You have been selected by " +
            empFirstName + " " + empLastName +
            " as a " + rater.Role + " for one of their evaluations. Please click the link below to complete the " +
            "following evaluation before the close date:" + "\r\n\r\n" +
            "Employee: " + empFirstName + " " + empLastName + "\r\n" +
            "Type: " + evaluation.Type.TypeName + "\r\n" + "Stage: " + evaluation.Stage.StageName + "\r\n" +
            "Open Date: " + evaluation.OpenDate.Date.ToString("d") + "\r\n" +
            "Close Date: " + evaluation.CloseDate.Date.ToString("d") + "\r\n\r\n" +
            "Click <a href=\"" + callbackUrl + "\">here</a> to evaluate " + empFirstName + " " + empLastName + ".";

            UserManager.SendEmail(userAccount.Id, emailSubject, emailBody);
        }

        private ApplicationUser GenerateTemporaryRaterAccount(int raterId)
        {
            var rater = UnitOfWork.RaterRepository.GetByID(raterId);
            if (UserManager.Users.Any(u => u.UserName.Equals(rater.Email)))
            {
                return UserManager.FindByEmail(rater.Email);               
            }

            var user = new ApplicationUser
            {
                UserName = rater.Email,
                Email = rater.Email,
                EmailConfirmed = true
            };

            var result = UserManager.Create(user);
            if (result.Succeeded)
            {
                UserManager.AddToRole(user.Id, "Rater");
            }

            return user;
        }
    }
}