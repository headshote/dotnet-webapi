using System.Net.Http;
using Microsoft.Web.Http.Versioning;

namespace WebApplicationExercise.Infrastructure.Errors
{
    public class VersioningErrorResponseProvider : DefaultErrorResponseProvider
    {
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

            return base.CreateResponse(context);
        }
    }
}