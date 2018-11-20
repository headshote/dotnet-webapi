using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Ajax.Utilities;

namespace WebApplicationExercise.Web.Filters
{
    public class InputDateToUtcConversionFilter : ActionFilterAttribute, IActionFilter
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var dates = new Dictionary<String, DateTime>();
            actionContext.ActionArguments.ForEach(a =>
            {
                if (a.Value is DateTime)
                {
                    dates[a.Key] = DateTime.SpecifyKind((DateTime)a.Value, DateTimeKind.Utc);
                }
            });


            dates.ForEach(a => actionContext.ActionArguments[a.Key] = a.Value);
        }
    }
}