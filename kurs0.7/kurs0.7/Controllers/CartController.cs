using kurs0._7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace kurs0._7.Controllers
{
    public class CartController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger("CartController");
        private Entities db = new Entities();

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public PartialViewResult CheckCart(Cart cart, string returnUrl)
        {

            return PartialView(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = null
            });
        }


        public RedirectToRouteResult AddToCart(Cart cart, int id, string returnUrl)
        { 
            Materials matertial = db.Materials.FirstOrDefault(m => m.Id == id);

            if (matertial != null)
            {
                cart.AddItem(matertial, 1);
            }
            else
            {
                logger.Warn("Не удалось добавить в корзину материал, с id = "+ id + ", возможно он был удален");
            }
            

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart,int id, string returnUrl)
        {
            Materials matertial = db.Materials.FirstOrDefault(m => m.Id == id);

            if (matertial != null)
            {
                cart.RemoveLine(matertial);
            }
            else
            {
                logger.Warn("Не удалось удалить из корзины материал, с id = " + id + ", возможно он был удален");
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ActionResult Checkout(Cart cart, string returnUrl, ClientDetails clientDetails)
        {
            if(cart.Lines.Count() != 0)
            {
                return View(new ClientDetails());
            }
            return RedirectToAction("Index", new { returnUrl });
        }
    }
}