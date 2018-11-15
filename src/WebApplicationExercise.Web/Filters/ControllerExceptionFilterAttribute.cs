using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http.Filters;
using Unity.Attributes;
using WebApplicationExercise.Core.Interfaces;

namespace WebApplicationExercise.Web.Filters
{
    public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
    {
        [Dependency]
        public ILogger Logger { get; set; }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            var controlleName = context.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = context.ActionContext.ActionDescriptor.ActionName;
            var request = context.Request;

            ThreadPool.QueueUserWorkItem(task => Logger.Information("Exception {0} occured int the method '{1}' of the controller '{2}'. Requestdata: {3}",
                context.Exception,
                methodName, 
                controlleName,
                request));
        }
    }
}