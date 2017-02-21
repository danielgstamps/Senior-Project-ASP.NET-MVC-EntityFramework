using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using LumenWorks.Framework.IO.Csv;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private DataContext db = new DataContext();
        private DataTable csvTable = new DataTable();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Cohort).Include(e => e.Manager).Include(e => e.Supervisor);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        
        public ActionResult UploadData()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadData(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {

                    if (upload.FileName.EndsWith(".csv"))
                    {
                        var stream = upload.InputStream;
                        using (var csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            csvTable.Load(csvReader);
                        }
                        insertCSVDataIntoDB();
                        return View(csvTable);
                    }
                    ModelState.AddModelError("File", "This file format is not supported.\r\n\r\nPlease upload a .csv file.");
                    return View();
                }
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            return View();
        }

        private void insertCSVDataIntoDB()
        {
            for(var i = 0; i < csvTable.Rows.Count; i++)
            {
                var e1 = new Employee
                {
                    FirstName = csvTable.Rows[i][0].ToString(),
                    LastName = csvTable.Rows[i][1].ToString(),
                    Email = csvTable.Rows[i][2].ToString(),
                    Address = csvTable.Rows[i][2].ToString(),
                    Phone = csvTable.Rows[i][3].ToString()
                };
                db.Employees.Add(e1);
                db.SaveChanges();
            }
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.CohortID = new SelectList(db.Cohorts, "CohortID", "Name");
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "FirstName");
            ViewBag.SupervisorID = new SelectList(db.Employees, "EmployeeID", "FirstName");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,FirstName,LastName,Email,Address,Phone,CohortID,SupervisorID,ManagerID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CohortID = new SelectList(db.Cohorts, "CohortID", "Name", employee.CohortID);
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "FirstName", employee.ManagerID);
            ViewBag.SupervisorID = new SelectList(db.Employees, "EmployeeID", "FirstName", employee.SupervisorID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.CohortID = new SelectList(db.Cohorts, "CohortID", "Name", employee.CohortID);
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "FirstName", employee.ManagerID);
            ViewBag.SupervisorID = new SelectList(db.Employees, "EmployeeID", "FirstName", employee.SupervisorID);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,FirstName,LastName,Email,Address,Phone,CohortID,SupervisorID,ManagerID")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CohortID = new SelectList(db.Cohorts, "CohortID", "Name", employee.CohortID);
            ViewBag.ManagerID = new SelectList(db.Employees, "EmployeeID", "FirstName", employee.ManagerID);
            ViewBag.SupervisorID = new SelectList(db.Employees, "EmployeeID", "FirstName", employee.SupervisorID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
