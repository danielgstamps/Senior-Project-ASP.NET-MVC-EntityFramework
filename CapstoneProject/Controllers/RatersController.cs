using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CapstoneProject.Controllers
{
    public class RatersController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _userDb = new ApplicationDbContext();

        public IUnitOfWork UnitOfWork { get; set; } = new UnitOfWork();
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
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

            TempData["EmailSuccess"] = "Sent notification email to " + rater.Name;
            return RedirectToAction("EditRaters", "Evaluations", new { id = evalId });
        }

        private void SendRaterEmail(int raterId, int evalId)
        {
            var rater = UnitOfWork.EmployeeRepository.GetByID(raterId);
            var userAccount = GenerateTemporaryRaterAccount(raterId);
            var evaluation = UnitOfWork.EvaluationRepository.GetByID(evalId);

            var callbackUrl = Url.Action("TakeEvaluation", "Evaluations", new { id = evaluation.EvaluationID }, Request.Url.Scheme);
            var emailSubject = "Rater Email";

            var emailBody =
            "You have a new evaluation to complete: " +
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

            UserManager.SendEmail(userAccount.Id, emailSubject, emailBody);
        }

        private ApplicationUser GenerateTemporaryRaterAccount(int raterId)
        {
            var rater = UnitOfWork.RaterRepository.GetByID(raterId);
            var user = new ApplicationUser
            {
                UserName = rater.Email,
                Email = rater.Email,
                EmailConfirmed = true
            };

            const string userPwd = "123123"; // If they do have to log in, change this to randomly generated string.

            // Write user to DB.
            UserManager.Create(user, userPwd);
            return user;
        }
    }
}