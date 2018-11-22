using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplicationExercise.Infrastructure.Errors.Results
{
    public class ErrorActionResult : IHttpActionResult
    {
        public HttpResponseMessage Response { get; set; }

        public ErrorActionResult(HttpResponseMessage originalResponse, HttpContent newContent = null)
        {
            Response = originalResponse;

            if (newContent != null)
            {
                Response.Content = newContent;
            }
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }
    }
}
