using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using kurs0._7;
using kurs0._7.Models;
using log4net;

namespace kurs0._7.Controllers
{
    public class OrdersController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger("OrdersController");
        private Entities db = new Entities();

        public ActionResult Confirm(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            orders.State = 2;
            foreach (var item in db.OrderItems.Where(oi => oi.OrderNumber == orders.Id))
            {
                Materials mat = db.Materials.Find(item.Material);
                mat.Reserved -= item.Count;
                mat.Count -= item.Count;
                if (mat.Reserved > mat.Count)
                {
                    return View("CantEditState");
                }
                db.Entry(mat).State = EntityState.Modified; 
            }
            db.Entry(orders).State = EntityState.Modified;
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

        public ActionResult Reject(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            orders.State = 3;
            foreach (var item in db.OrderItems.Where(oi => oi.OrderNumber == orders.Id))
            {
                Materials mat = db.Materials.Find(item.Material);
                mat.Reserved -= item.Count;
                db.Entry(mat).State = EntityState.Modified;

            }
            db.Entry(orders).State = EntityState.Modified;
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

        // GET: Orders
        [Authorize (Roles = "Manager,Administrator")]
        public ActionResult Index()
        {
            return View(db.Orders.ToList());
        }


        // GET: Orders
        [Authorize]
        public ActionResult UserOrders(string uname)
        {
            return PartialView(db.Orders.Where(o => o.Client == uname).ToList());
        }

        // GET: Orders/Details/5
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.Error("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(orders);
        }

        //GET: OrderItems
        [Authorize]
        public ActionResult Items(string id)
        {
            return PartialView(db.OrderItems.Where(oi => oi.OrderNumber == id).ToList());
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Telephone, EMail")] ClientDetails clientDetails, Cart cart)
        {
            Orders orders = new Orders();
            if (ModelState.IsValid)
            {
                string id = String.Format(@"{0}", System.Guid.NewGuid());
                orders.Client = clientDetails.EMail;
                orders.Telephone = clientDetails.Telephone;
                orders.Date = DateTime.Now;
                orders.State = 1;
                orders.Id = id;
                orders.Price = cart.ComputeTotalValue();

                db.Orders.Add(orders);
                foreach (var item in cart.Lines)
                {
                    OrderItems orderItems = new OrderItems();
                    orderItems.OrderNumber = id;
                    orderItems.Count = item.Quantity;
                    orderItems.Material = item.material.Id;
                    Materials mat = db.Materials.Find(item.material.Id);
                    mat.Reserved += item.Quantity;
                    if(mat.Reserved > mat.Count)
                    {
                        return View("CantCreate");
                    }

                    db.Entry(mat).State = EntityState.Modified;
                    db.OrderItems.Add(orderItems);
                }

                try
                {
                    db.SaveChanges();
                    cart.Clear();
                }
                catch (Exception ex)
                {
                    if (logger.IsErrorEnabled)
                    {
                        logger.Error("Невозможно сохранить данные", ex);
                    }
                }
                
                return RedirectToAction("Index", "Manage", null);
            }

            return View(orders);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Manager,Administrator")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            ViewBag.State = new SelectList(db.States, "Id", "Name", orders.State);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Manager,Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Client,Date,State,Telephone,Price")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
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
            return View(orders);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Manager,Administrator")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                if (logger.IsWarnEnabled)
                {
                    logger.Warn("SQL запрос не выдал результатов по идентификатору: " + id);
                }
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager,Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Orders orders = db.Orders.Find(id);

            switch (orders.State)
            {
                case 2:
                    foreach (var item in db.OrderItems.Where(oi => oi.OrderNumber == orders.Id).ToList())
                    {
                        Materials mat = db.Materials.Find(item.Material);
                        mat.Count += item.Count;
                        db.Entry(mat).State = EntityState.Modified;
                        db.OrderItems.Remove(item);
                    }
                    break;
                case 3:
                    foreach (var item in db.OrderItems.Where(oi => oi.OrderNumber == orders.Id).ToList())
                    {
                        db.OrderItems.Remove(item);
                    }
                    break;
                default:
                    foreach (var item in db.OrderItems.Where(oi => oi.OrderNumber == orders.Id).ToList())
                    {
                        Materials mat = db.Materials.Find(item.Material);
                        mat.Reserved -= item.Count;
                        db.Entry(mat).State = EntityState.Modified;
                        db.OrderItems.Remove(item);
                    }
                    break;

            }
            db.Orders.Remove(orders);
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