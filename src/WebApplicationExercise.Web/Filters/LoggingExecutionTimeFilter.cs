using System.Diagnostics;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unity.Attributes;
using WebApplicationExercise.Core.Interfaces;

namespace WebApplicationExercise.Web.Filters
{
    public class LoggingExecutionTimeFilter : ActionFilterAttribute, IActionFilter
    {
        [Dependency]
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            actionContext.ActionArguments["stopwatch"] = stopwatch;

            var controlleName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionDescriptor.ActionName;
            ThreadPool.QueueUserWorkItem(task => Logger.Information("Method {0} of the controller {1} started execution.", methodName, controlleName));
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!(actionExecutedContext.ActionContext.ActionArguments["stopwatch"] is Stopwatch stopwatch))
            {
                return;
            }
            stopwatch.Stop();

            var controlleName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            ThreadPool.QueueUserWorkItem(task => Logger.Information("Method {0} of the controller {1} finished execution after running for {2}.", methodName, controlleName, stopwatch.Elapsed));
        }
    }
}
