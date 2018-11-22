using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplicationExercise.Infrastructure.Errors.Results
{
    public class ErrorActionResult : IHttpActionResult
    {
        public HttpResponseMessage Response { get; set; }

        public ErrorActionResult(HttpResponseMessage response, string extraMessage=null)
        {
            Response = response;

            Response.Content = new ObjectContent<HttpError>(HttpErrorFormatGenerator.CreateError(response.Content, extraMessage), 
                new JsonMediaTypeFormatter());
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }
    }
}
