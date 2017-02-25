using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;

namespace CapstoneProject.Controllers
{
    public class CohortsController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Cohorts
        public ActionResult Index()
        {
            return View(db.Cohorts.ToList());
        }

        // GET: Cohorts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = db.Cohorts.Find(id);
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
                db.Cohorts.Add(cohort);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cohort);
        }

        // GET: Cohorts/Edit/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = db.Cohorts.Find(id);
            var employeesUnordered = db.Employees.ToList();
            var employeesOrdered = employeesUnordered.OrderBy(e => e.LastName).ToList();
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
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedEmployees/*[Bind(Include = "CohortID,Name")] Cohort cohort*/)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohortToUpdate = db.Cohorts.Include(c => c.Employees).Where(c => c.CohortID == id).Single();
            if (TryUpdateModel(cohortToUpdate, "",
               new string[] { "Name" }))
            {
                try
                {
                    updateCohortEmployees(selectedEmployees, cohortToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            populateAssignedEmployees(cohortToUpdate);
            return View(cohortToUpdate);
        }

        private void updateCohortEmployees(string[] selectedEmployees, Cohort cohortToUpdate)
        {
            if (selectedEmployees == null)
            {
                cohortToUpdate.Employees = new List<Employee>();
                return;
            }

            var selectedEmployeesHashSet = new HashSet<string>(selectedEmployees);
            var cohortEmployees = new HashSet<int>
                (cohortToUpdate.Employees.Select(e => e.EmployeeID));
            foreach (var employee in db.Employees)
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
            var allEmployees = db.Employees;
            var cohortEmployees = new HashSet<int>(cohort.Employees.Select(e => e.EmployeeID));
            var assignedEmployee = new List<AssignedEmployeeData>();
            foreach (var employee in allEmployees)
            {
                assignedEmployee.Add(
                    new AssignedEmployeeData()
                    {
                        Employee = employee,
                        Assigned = cohortEmployees.Contains(employee.EmployeeID)
                    });
            }
            ViewBag.Employees = assignedEmployee;
        }

        // GET: Cohorts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cohort = db.Cohorts.Find(id);
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
            var cohort = db.Cohorts.Find(id);
            db.Cohorts.Remove(cohort);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
