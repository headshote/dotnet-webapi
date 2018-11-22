using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using Microsoft.Web.Http.Versioning;

namespace WebApplicationExercise.Infrastructure.Errors.Results
{
    public class HttpErrorFormatGenerator
    {
        public static HttpError CreateError(HttpActionExecutedContext context)
        {
            return CreateOuterErrorObject(new HttpError()
            {
                Message = "An error occurred, please try again or contact the administrator."
            });
        }

        public static HttpError CreateError(ExceptionHandlerContext errorContext)
        {
            return CreateOuterErrorObject(new HttpError()
            {
                Message = errorContext.ExceptionContext.Exception.Message
            });
        }

        public static HttpError CreateError(ErrorResponseContext context)
        {
            var innerError = new HttpError();

            if (!string.IsNullOrEmpty(context.ErrorCode))
            {
                innerError["Code"] = context.ErrorCode;
            }

            if (!string.IsNullOrEmpty(context.Message))
            {
                innerError.Message = context.Message;
            }

            if (!string.IsNullOrEmpty(context.MessageDetail) && context.Request.ShouldIncludeErrorDetail() == true)
            {
                innerError["ErrorDetail"] = new HttpError(context.MessageDetail);
            }

            return CreateOuterErrorObject(innerError);
        }

        public static HttpError CreateError(HttpContent content, string extraMessage)
        {
            HttpError innerError = content != null ? content.ReadAsAsync<HttpError>().Result : new HttpError();

            if (!string.IsNullOrEmpty(extraMessage))
            {
                innerError["ErrorDetail"] = extraMessage;
            }

            return CreateOuterErrorObject(innerError);
        }

        private static HttpError CreateOuterErrorObject(HttpError innerError)
        {
            return new HttpError()
            {
                ["Error"] = innerError
            };
        }
    }
}
