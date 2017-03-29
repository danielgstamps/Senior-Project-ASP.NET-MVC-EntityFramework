using CapstoneProject.DAL;
using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CapstoneProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Questions
        public ActionResult Index()
        {
            return View(unitOfWork.QuestionRepository.Get());
        }

        // GET: Questions/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var question = db.Questions.Find(id);
        //    if (question == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View("Details", question);
        //}

        // GET: Questions/Create
        public ActionResult Create()
        {
            QuestionViewModel model = new QuestionViewModel();
            model.CategoryList = unitOfWork.CategoryRepository.dbSet.Select(c => new SelectListItem()
            {
                Value = c.CategoryID.ToString(),
                Text = c.Name
            });
            return View(model);
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestionText,CategoryID")] QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                Question question = new Question()
                {
                    QuestionText = model.QuestionText,
                    Category = unitOfWork.CategoryRepository.GetByID(model.CategoryID)
                };

                unitOfWork.QuestionRepository.Insert(question);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Questions/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Questions/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
