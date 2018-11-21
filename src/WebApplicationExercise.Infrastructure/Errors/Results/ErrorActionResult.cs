using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplicationExercise.Infrastructure.Errors.Results
{
    public class ErrorActionResult : IHttpActionResult
    {
        public HttpResponseMessage Content { get; set; }

        public ErrorActionResult(HttpResponseMessage content)
        {
            Content = content;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Content);
        }
    }
}
