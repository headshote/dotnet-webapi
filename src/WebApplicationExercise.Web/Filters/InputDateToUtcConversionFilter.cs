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
            var dates = actionContext.ActionArguments.Where(a => a.Value is DateTime).ToList();

            dates.ForEach(a => actionContext.ActionArguments[a.Key] = DateTime.SpecifyKind((DateTime)a.Value, DateTimeKind.Utc));
        }
    }
}