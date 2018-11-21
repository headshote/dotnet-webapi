﻿using System;
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

        public async Task<IHttpActionResult> ConverErrorActionToInternalFormat(IHttpActionResult original)
        {
            return await ConverErrorActionToInternalFormat(original, null);
        }

        public async Task<IHttpActionResult> ConverErrorActionToInternalFormat(IHttpActionResult original, string extraErrorMessage)
        {
            var resultOriginal = await original.ExecuteAsync(new CancellationToken());

            ErrorActionResult result = new ErrorActionResult(resultOriginal);

            return result;
        }

        public IHttpActionResult CreateErrorAction(ExceptionHandlerContext errorContext)
        {
            var content = errorContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError()
            {
                ["Error"] = new HttpError()
                {
                    Message = errorContext.ExceptionContext.Exception.Message
                }
            });

            return new ErrorActionResult(content);
        }

        public HttpResponseMessage CreateErrorMessage(ErrorResponseContext errorContext)
        {
            return errorContext.Request.CreateErrorResponse(errorContext.StatusCode, createErrorContent(errorContext));
        }

        public HttpResponseMessage CreateErrorMessage(HttpActionExecutedContext actionContext)
        {
            return actionContext.Request.CreateErrorResponse(actionContext.Response?.StatusCode ?? HttpStatusCode.InternalServerError, createErrorContent(actionContext));
        }

        public void LogErrorDetails(ExceptionHandlerContext errorContext)
        {
            ThreadPool.QueueUserWorkItem(task => _logger.Information("Exception\n{0}\noccured during the method '{1}' of the controller '{2}'. Requestdata:\n{3}",
                errorContext.Exception,
                errorContext.ExceptionContext.ActionContext.ActionDescriptor.ActionName,
                errorContext.ExceptionContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
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

        private HttpError createErrorContent(HttpActionExecutedContext context)
        {
            var error = new HttpError();
            var root = new HttpError() { ["Error"] = error };
            error.Message = "An error occurred, please try again or contact the administrator.";
            return root;
        }

        private HttpError createErrorContent(ErrorResponseContext context)
        {
            var error = new HttpError();
            var root = new HttpError() { ["Error"] = error };

            if (!string.IsNullOrEmpty(context.ErrorCode))
            {
                error["Code"] = context.ErrorCode;
            }

            if (!string.IsNullOrEmpty(context.Message))
            {
                error.Message = context.Message;
            }

            if (!string.IsNullOrEmpty(context.MessageDetail) && context.Request.ShouldIncludeErrorDetail() == true)
            {
                error["ErrorDetail"] = new HttpError(context.MessageDetail);
            }

            return root;
        }
    }
}
