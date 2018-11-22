using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplicationExercise.Infrastructure.Errors.Results;

namespace WebApplicationExercise.Infrastructure.Errors
{
    public class ErrorHandler : DelegatingHandler
    {
        private readonly IErrorManager _errorManager;

        public ErrorHandler(IErrorManager errorManager)
        {
            _errorManager = errorManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = response.Content != null ? await response.Content.ReadAsAsync<HttpError>(cancellationToken) : null;
                if (!(content?.ContainsKey("Error") ?? false))
                {
                    response.Content = _errorManager.ConvertErrorContentToInternalFormat(response.Content);
                }
            }

            return response;
        }
    }
}
