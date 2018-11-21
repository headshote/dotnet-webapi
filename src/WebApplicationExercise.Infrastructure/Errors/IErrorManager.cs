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
        Task<IHttpActionResult> ConverErrorActionToInternalFormat(IHttpActionResult original);
        Task<IHttpActionResult> ConverErrorActionToInternalFormat(IHttpActionResult original, string extraErrorMessage);

        IHttpActionResult CreateErrorAction(ExceptionHandlerContext errorContext);

        HttpResponseMessage CreateErrorMessage(ErrorResponseContext errorContext);
        HttpResponseMessage CreateErrorMessage(HttpActionExecutedContext actionContext);

        void LogErrorDetails(ExceptionHandlerContext errorContext);
        void LogErrorDetails(HttpActionExecutedContext actionContext);
    }
}
