using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private DataContext db = new DataContext();
        private DataTable csvTable = new DataTable();
        private ApplicationDbContext dbUser = new ApplicationDbContext();
      //  private AccountController accountController = new AccountController();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public EmployeesController()
        {
        }

        public EmployeesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

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
        public async Task<ActionResult> UploadData(HttpPostedFileBase upload)
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
                        await InsertCsvDataIntoDb();
                        return View(csvTable);
                    }
                    ModelState.AddModelError("File", "This file format is not supported.\r\n\r\nPlease upload a .csv file.");
                    return View();
                }
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            return View();
        }

        private async Task InsertCsvDataIntoDb()
        {
            string duplicates = "";
            for(var i = 0; i < csvTable.Rows.Count; i++)
            {
                bool isDuplicate = false;
                string firstName = csvTable.Rows[i][0].ToString();
                string lastName = csvTable.Rows[i][1].ToString();
                string email = csvTable.Rows[i][2].ToString();
                string address = csvTable.Rows[i][3].ToString();
                string phone = csvTable.Rows[i][4].ToString();

                if (db.Employees.Any(e => e.Email.Equals(email)) ||
                    dbUser.Users.Any(u => u.Email.Equals(email)))
                {
                    isDuplicate = true;
                }

                // Remove duplicate emails from displayed table. Will need to change if csv format changes (if email is moved).
                if (isDuplicate)
                {
                    csvTable.Rows.Remove(csvTable.Rows[i]);
                    duplicates += firstName + " " + lastName + ", ";
                    i--; // Since row[0] was just deleted, row[1] became row[0], so move i back.
                    continue;
                }

                var e1 = new Employee
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Address = address,
                    Phone = phone
                };
                var u1 = new ApplicationUser
                {
                    Email = e1.Email,
                    UserName = e1.Email,
                    PhoneNumber = e1.Phone
                };

                dbUser.Users.Add(u1);
                db.Employees.Add(e1);
                dbUser.SaveChanges();
                db.SaveChanges();
                await SendPasswordCreationEmail(u1);
            }

            if (!string.IsNullOrEmpty(duplicates))
            {
                ViewBag.Duplicates = "The following duplicates were skipped: " + 
                    duplicates.Substring(0, duplicates.Length - 2) + ".";
            }
        }

        private async Task SendPasswordCreationEmail(ApplicationUser user)
        {
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code },
                protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Create your WUDSCO password",
                "Click <a href=\"" + callbackUrl + "\">here</a> to create your password.");

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

            var aspNetUser = dbUser.Users.Where(a => a.Email.Equals(employee.Email)).Single();
            dbUser.Users.Remove(aspNetUser);

            db.SaveChanges();
            dbUser.SaveChanges();
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