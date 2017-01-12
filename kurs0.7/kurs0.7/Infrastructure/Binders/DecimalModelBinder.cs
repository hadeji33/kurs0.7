using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kurs0._7.Infrastructure.Binders
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            var valueProvederResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            return valueProvederResult == null ? base.BindModel(controllerContext, bindingContext) : Convert.ToDecimal(valueProvederResult.AttemptedValue);
        }
    }
}