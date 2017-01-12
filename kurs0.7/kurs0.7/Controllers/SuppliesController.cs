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
    public class SuppliesController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger("SuppliesController");
        private Entities db = new Entities();

        // GET: Supplies
        public ActionResult Index()
        {
            var supply = db.Supply.Include(s => s.Materials);
            return View(supply.ToList());
        }

        // GET: Supplies/Create
        [Authorize(Roles = "Administrator,Manager")]
        public ActionResult Create()
        {
            ViewBag.Material = new SelectList(db.Materials, "Id", "Name");
            return View();
        }

        // POST: Supplies/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Material,Count,")] Supply supply)
        {
            if (ModelState.IsValid)
            {
                supply.Date = DateTime.Now;
                Materials mat = db.Materials.Find(supply.Material);
                mat.Count += supply.Count;
                db.Entry(mat).State = EntityState.Modified;
                db.Supply.Add(supply);
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

            ViewBag.Material = new SelectList(db.Materials, "Id", "Name", supply.Material);
            return View(supply);
        }

        // GET: Supplies/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supply supply = db.Supply.Find(id);
            if (supply == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            ViewBag.price = supply.Count;
            ViewBag.Material = new SelectList(db.Materials, "Id", "Name", supply.Material);
            return View(supply);
        }

        // POST: Supplies/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Material,Count,Date")] Supply supply, int oldCount)
        {
            if (ModelState.IsValid)
            {
                Materials mat = db.Materials.Find(supply.Material);
                mat.Count -= oldCount;
                mat.Count += supply.Count;
                db.Entry(mat).State = EntityState.Modified;
                db.Entry(supply).State = EntityState.Modified;
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
            ViewBag.Material = new SelectList(db.Materials, "Id", "Name", supply.Material);
            return View(supply);
        }

        // GET: Supplies/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supply supply = db.Supply.Find(id);
            if (supply == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(supply);
        }

        // POST: Supplies/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supply supply = db.Supply.Find(id);
            db.Supply.Remove(supply);
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
