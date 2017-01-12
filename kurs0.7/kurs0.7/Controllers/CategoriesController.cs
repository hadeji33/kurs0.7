using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using kurs0._7;
using log4net;

namespace kurs0._7.Controllers
{
    public class CategoriesController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger("CategoriesController");
        private Entities db = new Entities();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                logger.Error("Не получен идентификатор при вызове метода. ");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(categories);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(categories);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error("Невозможно сохранить данные", ex);
                    }
                }
                return RedirectToAction("Index");
            }

            return View(categories);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                logger.Error("Не получен идентификатор при вызове метода. ");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: Categories/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categories).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error("Невозможно сохранить данные", ex);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Error("Не получен идентификатор при вызове метода. ");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categories categories = db.Categories.Find(id);
            db.Categories.Remove(categories);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("Невозможно сохранить данные", ex);
                }
            }
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
