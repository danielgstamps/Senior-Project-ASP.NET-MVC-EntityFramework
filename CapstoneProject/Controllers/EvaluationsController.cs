using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;

namespace CapstoneProject.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Evaluations
        public ActionResult Index()
        {
            var evaluations = db.Evaluations.Include(e => e.Employee);
            return View(evaluations.ToList());
        }

        // GET: Evaluations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evaluation = db.Evaluations.Find(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // GET: Evaluations/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName");
            return View("Create");
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
                db.Evaluations.Add(evaluation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", evaluation.EmployeeID);
            return View(evaluation);
        }

        // GET: Evaluations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evaluation = db.Evaluations.Find(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", evaluation.EmployeeID);
            return View(evaluation);
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
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", evaluation.EmployeeID);
            return View(evaluation);
        }

        // GET: Evaluations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evaluation = db.Evaluations.Find(id);
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
            var evaluation = db.Evaluations.Find(id);
            db.Evaluations.Remove(evaluation);
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
