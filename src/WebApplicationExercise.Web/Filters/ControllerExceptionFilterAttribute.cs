using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http.Filters;
using Unity.Attributes;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Infrastructure.Errors;

namespace WebApplicationExercise.Web.Filters
{
    public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
    {
        [Dependency]
        public IErrorManager ErrorManager { get; set; }

        public override void OnException(HttpActionExecutedContext context)
        {
            ErrorManager.LogErrorDetails(context);

            context.Response = ErrorManager.CreateErrorMessage(context);
        }
    }
}