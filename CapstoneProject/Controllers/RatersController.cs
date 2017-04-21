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

        public ApplicationSignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
        }

        // GET RaterPrompt
        public async Task<ActionResult> RaterPrompt(int? id, int? raterId, string code)
        {
            if (id == null || raterId == null || string.IsNullOrEmpty(code))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var rater = UnitOfWork.RaterRepository.GetByID(raterId);
            if (rater == null)
            {
                ViewBag.RaterError = "This evaluation is no longer available.";
                return View("ThankYou");
            }

            if (rater.Disabled)
            {
                ViewBag.RaterError = "This evaluation is no longer available.";
                return View("ThankYou");
            }

            if (!string.IsNullOrEmpty(rater.Answers))
            {
                ViewBag.RaterError = "You have already completed this evaluation.";
                return View("ThankYou");
            }

            var raterUser = UserManager.FindByName(rater.Email);
            var codeIsValid = UserManager.VerifyUserTokenAsync(raterUser.Id, "RaterLogin", code);

            if (codeIsValid.Result)
            {
                await SignInManager.SignInAsync(raterUser, false, false);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
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
            UserManager.Delete(raterUserAccount);

            return View("ThankYou");
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

            TempData["EmailSuccess"] = "Sent notification email to " + rater.Name;
            return RedirectToAction("EditRaters", "Evaluations", new { id = evalId });
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