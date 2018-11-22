using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using Microsoft.Web.Http.Versioning;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Infrastructure.Errors.Results;

namespace WebApplicationExercise.Infrastructure.Errors
{
    public class ErrorManager : IErrorManager
    {
        private readonly ILogger _logger;

        public ErrorManager(ILogger logger)
        {
            _logger = logger;
        }

        public IHttpActionResult ConverErrorActionToInternalFormat(IHttpActionResult original)
        {
            return ConverErrorActionToInternalFormat(original, null);
        }

        public IHttpActionResult ConverErrorActionToInternalFormat(IHttpActionResult original, string extraErrorMessage)
        {
            var resultOriginal = original.ExecuteAsync(new CancellationToken()).Result;

            return new ErrorActionResult(resultOriginal, extraErrorMessage);
        }

        public IHttpActionResult ConverErrorActionToInternalFormat(IHttpActionResult original, string fmt, params object[] vars)
        {
            return ConverErrorActionToInternalFormat(original, string.Format(fmt, vars));
        }

        public IHttpActionResult CreateErrorAction(ExceptionHandlerContext errorContext)
        {
            var response = errorContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, HttpErrorFormatGenerator.CreateError(errorContext));

            return new ErrorActionResult(response);
        }

        public HttpResponseMessage CreateErrorMessage(ErrorResponseContext errorContext)
        {
            return errorContext.Request.CreateErrorResponse(errorContext.StatusCode, HttpErrorFormatGenerator.CreateError(errorContext));
        }

        public HttpResponseMessage CreateErrorMessage(HttpActionExecutedContext actionContext)
        {
            return actionContext.Request.CreateErrorResponse(actionContext.Response?.StatusCode ?? HttpStatusCode.InternalServerError,
                HttpErrorFormatGenerator.CreateError(actionContext));
        }

        public void LogErrorDetails(ExceptionHandlerContext errorContext)
        {
            ThreadPool.QueueUserWorkItem(task => _logger.Information("Unhadled Exception occured\n{0}\nRequestdata:\n{1}",
                errorContext.Exception,
                errorContext.Request));
        }

        public void LogErrorDetails(HttpActionExecutedContext actionContext)
        {
            var controlleName = actionContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionContext.ActionDescriptor.ActionName;
            var request = actionContext.Request;

            ThreadPool.QueueUserWorkItem(task => _logger.Information("Exception\n{0}\noccured during the method '{1}' of the controller '{2}'. Requestdata:\n{3}",
                actionContext.Exception,
                methodName,
                controlleName,
                request));
        }
    }
}
