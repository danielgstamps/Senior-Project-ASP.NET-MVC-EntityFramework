using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;
using WebGrease.Css.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles="Admin")]
    public class CohortsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork = new UnitOfWork();
        private readonly ApplicationDbContext _userDb = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public IUnitOfWork UnitOfWork { get; set; } = new UnitOfWork();
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Cohorts
        public ActionResult Index()
        {
            return View("Index", this._unitOfWork.CohortRepository.Get().OrderBy(c => c.Name));
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = this._unitOfWork.CohortRepository.GetByID(id);
            if (cohort == null)
            {
                return HttpNotFound();
            }
            return View("Details", cohort);
        }

        [Authorize(Roles = "Admin")]
        // GET: Cohorts/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: Cohorts/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CohortID,Name")] Cohort cohort)
        {
            if (ModelState.IsValid)
            {
                this._unitOfWork.CohortRepository.Insert(cohort);
                this._unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(cohort);
        }

        // GET: Cohorts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cohort = this._unitOfWork.CohortRepository.GetByID(id);
            var allCohorts = this._unitOfWork.CohortRepository.Get();
            var allEmployees = this._unitOfWork.EmployeeRepository.Get();
            var employeesToShow = allEmployees.ToList();
            foreach (var currentCohort in allCohorts.ToList())
            {
                foreach (var currentEmployee in allEmployees.ToList())
                {
                    if (currentCohort.Employees.Contains(currentEmployee))
                    {
                        employeesToShow.Remove(currentEmployee);
                        if (cohort.Employees.Contains(currentEmployee))
                        {
                            employeesToShow.Add(currentEmployee);
                        }
                    }
                }
            }

            var employeesOrdered = employeesToShow.OrderBy(e => e.LastName).ToList();
            ViewBag.Employees = employeesOrdered;
            if (cohort == null)
            {
                return HttpNotFound();
            }
            return View("Edit", cohort);
        }

        // POST: Cohorts/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedEmployees)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohortToUpdate = _unitOfWork.CohortRepository.GetByID(id);
            if (TryUpdateModel(cohortToUpdate, "",
               new string[] { "Name" }))
            {
                try
                {
                    UpdateCohortEmployees(selectedEmployees, cohortToUpdate);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedEmployees(cohortToUpdate);
            return View(cohortToUpdate);
        }

        // GET: Cohorts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = this._unitOfWork.CohortRepository.GetByID(id);
            if (cohort == null)
            {
                return HttpNotFound();
            }
            return View("Delete", cohort);
        }

        // POST: Cohorts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cohort = _unitOfWork.CohortRepository.GetByID(id);
            var evalsToDelete = new List<Evaluation>();

            // Remove all cohort's employees from the cohort.
            foreach (var employee in cohort.Employees)
            {
                employee.CohortID = null;
                employee.Cohort = null;

                // Gather each employee's incomplete evals into evalsToDelete.
                foreach (var eval in employee.Evaluations)
                {
                    if (!eval.IsComplete())
                    {
                        evalsToDelete.Add(eval);
                    }
                }
            }

            // Delete each eval in evalsToDelete
            foreach (var eval in evalsToDelete)
            {
                _unitOfWork.EvaluationRepository.Delete(eval);
                _unitOfWork.Save();
            }

            _unitOfWork.CohortRepository.Delete(cohort);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public ActionResult NotifyCohort(int? cohortId, int? typeId)
        {
            if (cohortId == null || typeId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(cohortId);
            if (cohort == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employees = cohort.Employees.ToList();

            foreach (var emp in employees)
            {
                try
                {
                    var eval = emp.Evaluations.Single(e => !e.IsComplete() && e.TypeID == typeId);
                    SendEvaluationEmail(emp.EmployeeID, eval.EvaluationID);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            TempData["EmailSuccess"] = "Sent evaluation notifications to " + cohort.Name + ".";
            return RedirectToAction("Index", "Cohorts");
        }

        public async Task<ActionResult> SendCreatePasswordEmails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cohort = UnitOfWork.CohortRepository.GetByID(id);
            if (cohort == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            foreach (var employee in cohort.Employees)
            {
                var user = await UserManager.FindByNameAsync(employee.Email);
                if (user == null || user.EmailConfirmed)
                {
                    continue;
                }

                var email = user.Email;
                var callbackUrl = Url.Action("CreatePassword", "Account", new { userId = user.Id, email },
                    protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Create your WUDSCO password",
                    "Click <a href=\"" + callbackUrl + "\">here</a> to create your password.");
            }  

            TempData["EmailSuccess"] = "Sent notifications to pending accounts.";
            return RedirectToAction("Details", "Cohorts", new { id });
        }

        private void SendEvaluationEmail(int employeeId, int evalId)
        {
            var employee = UnitOfWork.EmployeeRepository.GetByID(employeeId);
            var userAccount = _userDb.Users.ToList().Find(u => u.Email.Equals(employee.Email));
            var evaluation = UnitOfWork.EvaluationRepository.GetByID(evalId);

            var emailSubject = "New Evaluation ";

            var emailBody = "Hello " + employee.FirstName + " " + employee.LastName + ". " +
                            "You have a new evaluation to complete: " +
                            "\r\n\r\n" +
                            "Type: " + evaluation.Type.TypeName +
                            "\r\n" +
                            "Stage: " + evaluation.Stage.StageName +
                            "\r\n" +
                            "Open Date: " + evaluation.OpenDate.Date.ToString("d") +
                            "\r\n" +
                            "Close Date: " + evaluation.CloseDate.Date.ToString("d") +
                            "\r\n\r\n";

            if (evaluation.OpenDate <= DateTime.Today.Date)
            {
                var callbackUrl = Url.Action("TakeEvaluation", "Evaluations", new {id = evaluation.EvaluationID}, Request.Url.Scheme);
                emailBody += "Click <a href=\"" + callbackUrl + "\">here</a> to complete your evaluation.";
                emailSubject += "Ready!";
            }
            else
            {
                var callbackUrl = Url.Action("EmployeeEvalsIndex", "Evaluations", new {id = evaluation.EmployeeID}, Request.Url.Scheme);
                emailBody += "Click <a href=\"" + callbackUrl + "\">here</a> to view all of your evaluations, and check back once they open.";
                emailSubject += " Opens " + evaluation.OpenDate.ToString("d");
            } 

            UserManager.SendEmail(userAccount.Id, emailSubject, emailBody);
        }

        private void UpdateCohortEmployees(string[] selectedEmployees, Cohort cohortToUpdate)
        {
            // case: All employees (or last employee) removed.
            if (selectedEmployees == null)
            {
                ResetCohort(cohortToUpdate);
                return;
            }

            var selectedEmployeesHashSet = new HashSet<string>(selectedEmployees);
            var cohortEmployees = new HashSet<int>
                (cohortToUpdate.Employees.Select(e => e.EmployeeID));

            // case: An employee is added.
            foreach (var employee in _unitOfWork.EmployeeRepository.Get())
            {
                if (selectedEmployeesHashSet.Contains(employee.EmployeeID.ToString()))
                {
                    if (cohortEmployees.Contains(employee.EmployeeID))
                    {
                        continue;
                    }

                    cohortToUpdate.Employees.Add(employee);
                    if (cohortToUpdate.Type1Assigned || cohortToUpdate.Type2Assigned)
                    {
                        CopyEvalToNewEmployee(cohortToUpdate, employee);
                    }                    
                }

                // case: An employee is removed.
                else
                {
                    if (!cohortEmployees.Contains(employee.EmployeeID))
                    {
                        continue;
                    }

                    cohortToUpdate.Employees.Remove(employee);
                    _unitOfWork.Save();

                    var evalsToDelete = employee.Evaluations.Where(e => !e.IsComplete()).ToList();
                    foreach (var eval in evalsToDelete)
                    {
                        _unitOfWork.EvaluationRepository.Delete(eval);
                        _unitOfWork.Save();       
                    }                    
                }
            }  
        }

        private void PopulateAssignedEmployees(Cohort cohort)
        {
            var allEmployees = this._unitOfWork.EmployeeRepository.Get();
            var cohortEmployees = new HashSet<int>(cohort.Employees.Select(e => e.EmployeeID));
            var assignedEmployees = new List<AssignedEmployeeData>();
            foreach (var employee in allEmployees)
            {
                assignedEmployees.Add(
                    new AssignedEmployeeData()
                    {
                        Employee = employee,
                        Assigned = cohortEmployees.Contains(employee.EmployeeID)
                    });
            }
            ViewBag.AssignedEmployees = assignedEmployees;
        }

        private void CopyEvalToNewEmployee(Cohort cohort, Employee employee)
        {
            var templateEvals = cohort.Employees.First().Evaluations.Where(e => !e.IsComplete());
            foreach (var eval in templateEvals)
            {
                employee.Evaluations.Add(new Evaluation()
                {
                    Type = eval.Type,
                    Stage = eval.Stage,
                    OpenDate = eval.OpenDate,
                    CloseDate = eval.CloseDate,
                    SelfAnswers = ""
                });
            }
            _unitOfWork.Save();
        }

        private void ResetCohort(Cohort cohort)
        {
            var employees = cohort.Employees;
            foreach (var employee in employees)
            {
                var evalsToDelete = employee.Evaluations.Where(e => !e.IsComplete()).ToList();
                foreach (var eval in evalsToDelete)
                {
                    _unitOfWork.EvaluationRepository.Delete(eval);
                    _unitOfWork.Save();
                }
            }

            cohort.Employees.Clear();
            cohort.Type1Assigned = false;
            cohort.Type2Assigned = false;
            _unitOfWork.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
