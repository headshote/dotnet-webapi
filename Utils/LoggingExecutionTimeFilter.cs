using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApplicationExercise.Logging;

namespace WebApplicationExercise.Utils
{
    public class LoggingExecutionTimeFilter : ActionFilterAttribute, IActionFilter
    {
        private readonly ILogger _logger = new Logger();
        private readonly Stopwatch _stopwatch;

        public LoggingExecutionTimeFilter()
        {
            _stopwatch = new Stopwatch();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _stopwatch.Start();

            var controlleName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionDescriptor.ActionName;
            _logger.Information("Method {0} of the controller {1} started execution.", methodName, controlleName);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _stopwatch.Stop();

            var controlleName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            _logger.TraceApi(controlleName, methodName, _stopwatch.Elapsed, "Execution finished.");
        }
    }
}