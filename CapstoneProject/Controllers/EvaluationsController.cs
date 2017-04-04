using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProject.ViewModels;

namespace CapstoneProject.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
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

        // GET: Evaluations
       // public ActionResult Index()
       // {
            //var eval = new Evaluation();

            //var q1 = new Question { QuestionID = 1, QuestionText = "I am never late for work." };
            //q1.Answers.Add(new Answer { AnswerID = 1, AnswerText = "Strongly Disagree" });
            //q1.Answers.Add(new Answer { AnswerID = 2, AnswerText = "Disagree" });
            //q1.Answers.Add(new Answer { AnswerID = 3, AnswerText = "Neutral" });
            //q1.Answers.Add(new Answer { AnswerID = 4, AnswerText = "Agree" });
            //q1.Answers.Add(new Answer { AnswerID = 5, AnswerText = "Strongly Agree" });
            //eval.Questions.Add(q1);

            //var q2 = new Question { QuestionID = 2, QuestionText = "I get along with my coworkers." };
            //q2.Answers.Add(new Answer { AnswerID = 6, AnswerText = "Strongly Disagree" });
            //q2.Answers.Add(new Answer { AnswerID = 7, AnswerText = "Disagree" });
            //q2.Answers.Add(new Answer { AnswerID = 8, AnswerText = "Neutral" });
            //q2.Answers.Add(new Answer { AnswerID = 9, AnswerText = "Agree" });
            //q2.Answers.Add(new Answer { AnswerID = 10, AnswerText = "Strongly Agree" });
            //eval.Questions.Add(q2);

            //var q3 = new Question { QuestionID = 3, QuestionText = "I complete projects early." };
            //q3.Answers.Add(new Answer { AnswerID = 11, AnswerText = "Strongly Disagree" });
            //q3.Answers.Add(new Answer { AnswerID = 12, AnswerText = "Disagree" });
            //q3.Answers.Add(new Answer { AnswerID = 13, AnswerText = "Neutral" });
            //q3.Answers.Add(new Answer { AnswerID = 14, AnswerText = "Agree" });
            //q3.Answers.Add(new Answer { AnswerID = 15, AnswerText = "Strongly Agree" });
            //eval.Questions.Add(q3);

           // var evals = unitOfWork.EvaluationRepository.Get();
            // return View("Index", eval);
       // }

        //[HttpPost]
        //public ActionResult Index(Evaluation model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        foreach (var q in model.Questions)
        //        {
        //            var qId = q.QuestionID;
        //            var selectedAnswer = q.SelectedAnswer;

        //            // Save the data 
        //        }
        //        return RedirectToAction("ThankYou"); //Should be changed to a send evaluation link
        //    }
        //    //to do : reload questions and answers
        //    return View(model);
        //}

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
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(this.unitOfWork.EmployeeRepository.Get(), "EmployeeID", "FirstName", evaluation.Employee);
            return View("Create", evaluation);
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
