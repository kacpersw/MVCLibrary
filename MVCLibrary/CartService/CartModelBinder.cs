using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCLibrary.CartService
{
    public class CartModelBinder : IModelBinder
    {
        //Session key
        private const string key = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Cart cart = null;
            //If session dict is not null - get cart from session dict
            if (controllerContext.HttpContext.Session != null)
            {
                cart = (Cart)controllerContext.HttpContext.Session[key];
            }
            //If there is no active cart instance, create new one
            if (cart == null && controllerContext.HttpContext.Session != null)
            {
                cart = new Cart();
                controllerContext.HttpContext.Session[key] = cart;
            }
            return cart;
        }
    }
}