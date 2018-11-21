using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Web.Http.Versioning;

namespace WebApplicationExercise.Infrastructure.Errors
{
    public class VersioningErrorResponseProvider : DefaultErrorResponseProvider
    {
        private readonly IErrorManager _errorManager;

        public VersioningErrorResponseProvider(IErrorManager errorManager)
        {
            _errorManager = errorManager;
        }

        public override HttpResponseMessage CreateResponse(ErrorResponseContext context)
        {
            switch (context.ErrorCode)
            {
                case "UnsupportedApiVersion":
                    context = new ErrorResponseContext(
                        context.Request,
                        context.StatusCode,
                        context.ErrorCode,
                        context.Message,
                        context.MessageDetail);
                    break;
            }

            return _errorManager.CreateErrorMessage(context);
            
        }
    }
}