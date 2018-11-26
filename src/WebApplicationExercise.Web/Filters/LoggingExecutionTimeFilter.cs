using System;
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
            actionContext.Request.Properties["stopwatch"] = stopwatch;

            var sessionId = Guid.NewGuid().ToString("D");
            actionContext.Request.Properties.Add("sessionid", sessionId);

            var controlleName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionDescriptor.ActionName;
            ThreadPool.QueueUserWorkItem(task => Logger.Information("[Request_{0}] Method {1} of the controller {2} started execution.", 
                sessionId, methodName, controlleName));
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!(actionExecutedContext.ActionContext.Request.Properties["stopwatch"] is Stopwatch stopwatch))
            {
                return;
            }
            stopwatch.Stop();

            var sessionId = actionExecutedContext.Request.Properties["sessionid"].ToString();

            var controlleName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            ThreadPool.QueueUserWorkItem(task => Logger.Information("[Request_{0}] Method {1} of the controller {2} finished execution after running for {3}.",
                sessionId, methodName, controlleName, stopwatch.Elapsed));
        }
    }
}
