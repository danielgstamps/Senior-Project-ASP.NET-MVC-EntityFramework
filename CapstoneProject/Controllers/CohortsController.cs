using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles="Admin")]
    public class CohortsController : Controller
    {
        private IUnitOfWork unitOfWork = new UnitOfWork();

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

        // GET: Cohorts
        public ActionResult Index()
        {
            return View("Index", this.unitOfWork.CohortRepository.Get());
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = this.unitOfWork.CohortRepository.GetByID(id);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CohortID,Name")] Cohort cohort)
        {
            if (ModelState.IsValid)
            {
                this.unitOfWork.CohortRepository.Insert(cohort);
                this.unitOfWork.Save();
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

            var cohort = this.unitOfWork.CohortRepository.GetByID(id);
            var allCohorts = this.unitOfWork.CohortRepository.Get();
            var allEmployees = this.unitOfWork.EmployeeRepository.Get();
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedEmployees/*[Bind(Include = "CohortID,Name")] Cohort cohort*/)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohortToUpdate = this.unitOfWork.CohortRepository.GetByID(id);
            if (TryUpdateModel(cohortToUpdate, "",
               new string[] { "Name" }))
            {
                try
                {
                    updateCohortEmployees(selectedEmployees, cohortToUpdate);

                    this.unitOfWork.Save();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    //throw new Exception("here");
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            populateAssignedEmployees(cohortToUpdate);
            return View(cohortToUpdate);
        }

        private bool CohortHasOpenEval(int cohortId)
        {
            var cohort = UnitOfWork.CohortRepository.GetByID(cohortId);
            var firstEmployee = cohort.Employees.First();
            if (firstEmployee == null)
            {
                return false;
            }
            var firstEval = firstEmployee.Evaluations.First();
            if (firstEval == null)
            {
                return false;
            }

            return firstEval.OpenDate < DateTime.Today && firstEval.CloseDate > DateTime.Today;
        }

        private void updateCohortEmployees(string[] selectedEmployees, Cohort cohortToUpdate)
        {
            if (selectedEmployees == null)
            {
                cohortToUpdate.Employees = new List<Employee>();
                cohortToUpdate.Employees.Clear();
                return;
            }

            var selectedEmployeesHashSet = new HashSet<string>(selectedEmployees);
            var cohortEmployees = new HashSet<int>
                (cohortToUpdate.Employees.Select(e => e.EmployeeID));
            foreach (var employee in this.unitOfWork.EmployeeRepository.Get())
            {
                if (selectedEmployeesHashSet.Contains(employee.EmployeeID.ToString()))
                {
                    if (!cohortEmployees.Contains(employee.EmployeeID))
                    {
                        cohortToUpdate.Employees.Add(employee);
                    }
                }
                else
                {
                    if (cohortEmployees.Contains(employee.EmployeeID))
                    {
                        cohortToUpdate.Employees.Remove(employee);
                    }
                }
            }
        }

        private void populateAssignedEmployees(Cohort cohort)
        {
            var allEmployees = this.unitOfWork.EmployeeRepository.Get();
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

        // GET: Cohorts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = this.unitOfWork.CohortRepository.GetByID(id);
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
            var cohort = this.unitOfWork.CohortRepository.GetByID(id);
            foreach (var employee in cohort.Employees)
            {
                employee.CohortID = null;
                employee.Cohort = null;
            }
            this.unitOfWork.CohortRepository.Delete(cohort);
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
    }
}
