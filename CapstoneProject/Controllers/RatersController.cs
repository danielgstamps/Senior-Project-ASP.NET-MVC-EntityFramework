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

            var callbackUrl = Url.Action("TakeEvaluation", "Evaluations", new { id = evaluation.EvaluationID }, Request.Url.Scheme);
            var emailSubject = "Evaluate Your Fellow Employee";

            var emailBody =
            "Hello " + rater.Name + "! You have been selected by " +
            empFirstName + " " + empLastName +
            " as a Rater for one of their evaluations. Please click the link below to complete the " +
            "following evaluation before the close date:" +
            "\r\n\r\n" +
            "Employee: " + empFirstName + " " + empLastName +
            "\r\n" +
            "Type: " + evaluation.Type.TypeName +
            "\r\n" +
            "Stage: " + evaluation.Stage.StageName +
            "\r\n" +
            "Open Date: " + evaluation.OpenDate.Date.ToString("d") +
            "\r\n" +
            "Close Date: " + evaluation.CloseDate.Date.ToString("d") +
            "\r\n\r\n" +
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

            const string userPwd = "123123"; // If they do have to log in, change this to randomly generated string?
            UserManager.Create(user, userPwd);
            return user;
        }
    }
}