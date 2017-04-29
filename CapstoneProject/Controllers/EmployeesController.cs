using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNet.Identity.Owin;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    [HandleError]
    public class EmployeesController : Controller
    {
        private IUnitOfWork unitOfWork = new UnitOfWork();
        private DataTable csvTable = new DataTable();
        private ApplicationDbContext dbUser = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public IUnitOfWork UnitOfWork {
            get
            {
                return this.unitOfWork;
            }
            set
            {
                this.unitOfWork = value;
            }
        }

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
            var employees = unitOfWork.EmployeeRepository.Get();
            return View("Index", employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = unitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View("Details", employee);
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
            var duplicates = "";
            for (var i = 0; i < csvTable.Rows.Count; i++)
            {
                var isDuplicate = false;
                var firstName = csvTable.Rows[i][0].ToString();
                var lastName = csvTable.Rows[i][1].ToString();
                var email = csvTable.Rows[i][2].ToString();
                var address = csvTable.Rows[i][3].ToString();
                var phone = csvTable.Rows[i][4].ToString();

                if (unitOfWork.EmployeeRepository.Get().Any(e => e.Email.Equals(email)) ||
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
                unitOfWork.EmployeeRepository.Insert(e1);
                dbUser.SaveChanges();
                unitOfWork.Save();
            }

            if (!string.IsNullOrEmpty(duplicates))
            {
                ViewBag.Duplicates = "The following duplicates were skipped: " +
                    duplicates.Substring(0, duplicates.Length - 2) + ".";
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = unitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            //ViewBag.CohortID = new SelectList(unitOfWork.CohortRepository.Get(), "CohortID", "Name", employee.CohortID);
            return View("Edit", employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,FirstName,LastName,Email,Address,Phone")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.EmployeeRepository.Update(employee);
                unitOfWork.Save();
                TempData["EditSuccess"] = "Edited Employee: " + employee.FirstName + " " + employee.LastName + ".";
                return RedirectToAction("Index");
            }
            //ViewBag.CohortID = new SelectList(unitOfWork.CohortRepository.Get(), "CohortID", "Name", employee.CohortID);
            return View("Edit", employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = unitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View("Delete", employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var employee = unitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }        

            var aspNetUser = dbUser.Users.Single(a => a.Email.Equals(employee.Email));
            dbUser.Users.Remove(aspNetUser);
            dbUser.SaveChanges();

            var cohort = unitOfWork.CohortRepository.GetByID(employee.CohortID); 
            if (cohort != null)
            {
                cohort.Employees.Remove(employee);
                UnitOfWork.Save();
                if (cohort.Employees.Count == 0)
                {
                    cohort.Type1Assigned = false;
                    cohort.Type2Assigned = false;
                }
            }

            unitOfWork.EmployeeRepository.Delete(employee);
            unitOfWork.Save();

            TempData["DeleteSuccess"] = "Deleted Employee: " + employee.FirstName + " " + employee.LastName + ".";
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> SendPasswordCreationEmail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = UnitOfWork.EmployeeRepository.GetByID(id);
            if (employee == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByNameAsync(employee.Email);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var email = user.Email;
            var callbackUrl = Url.Action("CreatePassword", "Account", new { userId = user.Id, email },
                protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Create your WUDSCO password",
                "Click <a href=\"" + callbackUrl + "\">here</a> to create your password.");

            TempData["EmailSuccess"] = "Sent notification email to " + employee.FirstName + " " + employee.LastName + ".";
            return RedirectToAction("Index", "Employees");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}