using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Unity.Attributes;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Infrastructure.Errors;
using WebApplicationExercise.Infrastructure.Migrations;

namespace WebApplicationExercise.Web.Filters
{
    public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
    {
        [Dependency]
        public IErrorManager ErrorManager { get; set; }

        private readonly Type _exceptionTypeToCheck;
        private readonly HttpStatusCode _returnCode;
        private readonly string _errorType;

        public ControllerExceptionFilterAttribute(Type exceptionTypeToCheck, HttpStatusCode returnCode, string errorType)
        {
            _exceptionTypeToCheck = exceptionTypeToCheck;
            _returnCode = returnCode;
            _errorType = errorType;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            ErrorManager.LogErrorDetails(context);

            if (context.Exception.GetType() == _exceptionTypeToCheck)
            {
                context.Response = ErrorManager.CreateErrorMessage(context, _returnCode, context.Exception.Message, _errorType);
            }
            else
            {
                context.Response = ErrorManager.CreateErrorMessage(context);
            }
        }
    }
}