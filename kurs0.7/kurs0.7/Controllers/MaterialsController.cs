using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using kurs0._7;
using System.IO;
using log4net;

namespace kurs0._7.Controllers
{
    public class MaterialsController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger("MaterialsController");
        private Entities db = new Entities();

        // GET: Categories
        public PartialViewResult Categories()
        {
            var categories = db.Categories;
            return PartialView(categories.ToList());
        }

        // GET: Materials
        public ActionResult Index(int? id)   
        {
            Categories category;
            if (id == null)
            {
                category = new Categories();
                category.Id = 0;
                category.Name = "Все товары";
            }
            else
            {
                category = db.Categories.Find(id);
            }
            if (category == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Details/5
        public ActionResult Category(int? id)
        {
            List<Materials> list = db.Materials.Include(m => m.Categories).ToList();
            if (id != 0)
            {
                list = db.Materials.Include(m => m.Categories).Where(m => (m.Category == id)).ToList();
            }
            if (list != null)
            {       
                foreach (Materials mat in list)
                {
                    if (mat.images == null) { mat.images = "/images/product.jpg"; }
                }
            }
            else
            {
                if (logger.IsErrorEnabled) { 
                logger.Error("Результаты не найдены по запросу материалов с категорией, соответствующей идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(list);
        }

        // GET: Materials/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materials materials = db.Materials.Find(id);
            if (materials == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(materials);
        }

        // GET: Materials/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Materials/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Category,Producer,Units,Price,Count,Reserved,images")] Materials materials)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase upload = Request.Files["photo"];
                if (upload != null && upload.ContentLength > 0)
                {
                    string directory = @"D:\Files\ТП\kurs\kurs0.7\kurs0.7\images";

                    var fileExtension = Path.GetExtension(upload.FileName);
                    var fileName = String.Format(@"{0}" + fileExtension, System.Guid.NewGuid());
                    upload.SaveAs(Path.Combine(directory, fileName));
                    materials.images = "/images/" + fileName;
                }
                else
                {
                    if (logger.IsWarnEnabled)
                    {
                        logger.Warn("При создании материала, не был загружен файл или файл является пустым. ");
                    }
                }
                db.Materials.Add(materials);
                try {
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error("Невозможно сохранить данные", ex);
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.Category = new SelectList(db.Categories, "Id", "Name", materials.Category);
            return View(materials);
        }

        // GET: Materials/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materials materials = db.Materials.Find(id);
            if (materials == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name", materials.Category);
            return View(materials);
        }

        // POST: Materials/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Category,Producer,Units,Price,Count,Reserved,images")] Materials materials)
        {
            if (ModelState.IsValid)
            {
                db.Entry(materials).State = EntityState.Modified;
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
            ViewBag.Category = new SelectList(db.Categories, "Id", "Name", materials.Category);
            return View(materials);
        }

        // GET: Materials/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Materials materials = db.Materials.Find(id);
            if (materials == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(materials);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Materials materials = db.Materials.Find(id);
            db.Materials.Remove(materials);
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
