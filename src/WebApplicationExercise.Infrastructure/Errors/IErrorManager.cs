using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using Microsoft.Web.Http.Versioning;

namespace WebApplicationExercise.Infrastructure.Errors
{
    public interface IErrorManager
    {
        IHttpActionResult ConvertErrorActionToInternalFormat(IHttpActionResult original);
        IHttpActionResult ConvertErrorActionToInternalFormat(IHttpActionResult original, string fmt, params object[] vars);
        IHttpActionResult ConvertErrorActionToInternalFormat(IHttpActionResult original, string extraErrorMessage);

        HttpContent ConvertErrorContentToInternalFormat(HttpContent original, string extraMessage = null);

        IHttpActionResult CreateErrorAction(ExceptionHandlerContext errorContext);

        HttpResponseMessage CreateErrorMessage(ErrorResponseContext errorContext);
        HttpResponseMessage CreateErrorMessage(HttpActionExecutedContext actionContext);
        HttpResponseMessage CreateErrorMessage(HttpActionExecutedContext actionContext, HttpStatusCode errorCode, string errorMessage, string errorType);

        void LogErrorDetails(ExceptionHandlerContext errorContext);
        void LogErrorDetails(HttpActionExecutedContext actionContext);
    }
}
