using System.Collections.Generic;
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
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Evaluations
        public ActionResult Index()
        {
            var eval = new Evaluation();

            var questions = new List<Question>();
            var q1 = new Question { QuestionID = 1, QuestionText = "I am never late for work." };
            var q2 = new Question { QuestionID = 2, QuestionText = "I get along with my coworkers." };
            var q3 = new Question { QuestionID = 3, QuestionText = "I complete projects early." };
            questions.Add(q1);
            questions.Add(q2);
            questions.Add(q3);

            foreach (var question in questions)
            {
                question.Answers.Add(new Answer { AnswerText = "Strongly Disagree" });
                question.Answers.Add(new Answer { AnswerText = "Disagree" });
                question.Answers.Add(new Answer { AnswerText = "Neutral" });
                question.Answers.Add(new Answer { AnswerText = "Agree" });
                question.Answers.Add(new Answer { AnswerText = "Strongly Agree" });
                eval.Questions.Add(question);
            }
            //var evaluations = db.Evaluations.Include(e => e.Employee);
            var evaluations = unitOfWork.EvaluationRepository.Get();
            return View(eval);
        }

        [HttpPost]
        public ActionResult Index(Evaluation model)
        {
            if (ModelState.IsValid)
            {
                foreach (var q in model.Questions)
                {
                    var qId = q.QuestionID;
                    var selectedAnswer = q.SelectedAnswer;
                    // Save the data 
                }
                return RedirectToAction("Index"); //Should be changed to a send evaluation link
            }
            //to do : reload questions and answers
            return View(model);
        }

        // GET: Evaluations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var evaluation = db.Evaluations.Find(id);
            var evaluation = unitOfWork.EvaluationRepository.GetByID(id);
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
                //db.Evaluations.Add(evaluation);
                //db.SaveChanges();
                this.unitOfWork.EvaluationRepository.Insert(evaluation);
                this.unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", evaluation.Employee);
            return View("Create", evaluation);
        }

        // GET: Evaluations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var evaluation = db.Evaluations.Find(id);
            var evaluation = unitOfWork.EvaluationRepository.GetByID(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", evaluation.Employee);
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
                //db.Entry(evaluation).State = EntityState.Modified;
                //db.SaveChanges();
                this.unitOfWork.EvaluationRepository.Update(evaluation);
                this.unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FirstName", evaluation.Employee);
            return View(evaluation);
        }

        // GET: Evaluations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var evaluation = db.Evaluations.Find(id);
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
            //var evaluation = db.Evaluations.Find(id);
            //db.Evaluations.Remove(evaluation);
            //db.SaveChanges();
            var evaluation = unitOfWork.EvaluationRepository.GetByID(id);
            this.unitOfWork.EvaluationRepository.Delete(evaluation);
            this.unitOfWork.Save();
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
