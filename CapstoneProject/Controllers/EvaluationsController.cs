using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;
using Microsoft.AspNet.Identity.Owin;

namespace CapstoneProject.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private IUnitOfWork unitOfWork = new UnitOfWork();
        private ApplicationDbContext userDB = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return this.unitOfWork;
            }
            set
            {
                this.unitOfWork = value;
            }
        }

        //GET: Evaluations
        public ActionResult Index()
        {
            var evaluations = unitOfWork.EvaluationRepository.Get();
            return View("Index", evaluations);
        }

        // GET: Evaluations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evaluation = unitOfWork.EvaluationRepository.GetByID(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View("Details", evaluation);
        }

        // GET: Evaluations/Create
        public ActionResult Create(int? cohortId)
        {
            if (cohortId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cohort = unitOfWork.CohortRepository.GetByID(cohortId);
            if (cohort == null)
            {
                return HttpNotFound();
            }

            EvaluationCreateViewModel model = new EvaluationCreateViewModel();
            model.CohortToEvaluate = cohort;
            model.TypeList = unitOfWork.TypeRepository.dbSet.Select(t => new SelectListItem()
            {
                Value = t.TypeID.ToString(),
                Text = t.TypeName,
            });

            if (model.CohortToEvaluate.Type1Assigned)
            {
                model.TypeList.ToList().Remove(model.TypeList.First());
            }

            if (model.CohortToEvaluate.Type2Assigned)
            {
                model.TypeList.Last().Disabled = true;
            }

            model.StageList = unitOfWork.StageRepository.dbSet.Select(t => new SelectListItem()
            {
                Value = t.StageID.ToString(),
                Text = t.StageName
            });

            return View("Create", model);
        }

        // POST: Evaluations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EvaluationID,Stage,Type,EmployeeID")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                this.unitOfWork.EvaluationRepository.Insert(evaluation);
                this.unitOfWork.Save();
                var cohortID = ViewBag.CohortID;
                this.sendEvaluationEmail(cohortID, evaluation);
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(this.unitOfWork.EmployeeRepository.Get(), "EmployeeID", "FirstName", evaluation.Employee);
            return View("Create", evaluation);
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        private async Task sendEvaluationEmail(int cohortID, Evaluation evaluation)
        {
            var cohort = this.unitOfWork.CohortRepository.GetByID(cohortID);
            var employees = cohort.Employees.ToList();
            var userAccounts = userDB.Users.ToList();
            foreach (var employee in employees)
            {
                var userAccount = userAccounts.Find(u => u.Email == employee.Email);
                var userEmail = userAccount.Email;

                // TODO Specify EvaluationsController Action in first string param
                var callbackUrl = Url.Action("CompleteEvaluation", "Evaluations", new { userId = userAccount.Id, email = userEmail }, protocol: Request.Url.Scheme);

                var emailSubject = "New Evaluation";
                var emailBody =
                "You have a new evaluation to complete. Here are the details: " +
                "\r\n\r\n" +
                "Type: " + evaluation.Type.TypeName + 
                "\r\n\r\n" + 
                "Stage: " + evaluation.Stage.StageName + 
                //"\r\n\r\n" + 
                //"Open Date: " + evaluation.OpenDate + 
                //"\r\n\r\n" + 
                //"Close Date: " + evaluation.ClosedDate + 
                "\r\n\r\n" + 
                "Click <a href=\"" + callbackUrl + "\">here</a> to complete your evaluation.";

                await UserManager.SendEmailAsync(userAccount.Id, emailSubject, emailBody);
            }
        }

        public ActionResult CompleteEvaluation()
        {
            return null;
        }

        // GET: Evaluations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evaluation = unitOfWork.EvaluationRepository.GetByID(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(this.unitOfWork.EmployeeRepository.Get(), "EmployeeID", "FirstName", evaluation.Employee);
            return View("Edit", evaluation);
        }

        // POST: Evaluations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EvaluationID,Stage,Type,EmployeeID")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                this.unitOfWork.EvaluationRepository.Update(evaluation);
                this.unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(this.unitOfWork.EmployeeRepository.Get(), "EmployeeID", "FirstName", evaluation.Employee);
            return View(evaluation);
        }

        // GET: Evaluations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evaluation = this.unitOfWork.EvaluationRepository.GetByID(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // POST: Evaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var evaluation = unitOfWork.EvaluationRepository.GetByID(id);
            this.unitOfWork.EvaluationRepository.Delete(evaluation);
            this.unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ThankYou()
        {
            return Content("Thank You");
        }
    }
}
