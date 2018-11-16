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
            var controlleName = context.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = context.ActionContext.ActionDescriptor.ActionName;
            var request = context.Request;

            ThreadPool.QueueUserWorkItem(task => Logger.Information("Exception {0}\noccured int the method '{1}' of the controller '{2}'. Requestdata: {3}",
                context.Exception,
                methodName, 
                controlleName,
                request));

            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An error occurred, please try again or contact the administrator.")
            };
        }
    }
}