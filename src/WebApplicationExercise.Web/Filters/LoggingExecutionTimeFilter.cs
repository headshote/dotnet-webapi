using System.Diagnostics;
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

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _stopwatch.Restart();

            var controlleName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionDescriptor.ActionName;
            Logger.Information("Method {0} of the controller {1} started execution.", methodName, controlleName);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _stopwatch.Stop();

            var controlleName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            Logger.Information("Method {0} of the controller {1} finished execution after running for {2}.", methodName, controlleName, _stopwatch.Elapsed);
        }
    }
}
