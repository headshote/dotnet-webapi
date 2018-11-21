using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace WebApplicationExercise.Infrastructure.Errors
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private readonly IErrorManager _errorManager;

        public GlobalExceptionHandler(IErrorManager errorManager)
        {
            _errorManager = errorManager;
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return base.ShouldHandle(context);
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            _errorManager.LogErrorDetails(context);
            context.Result = _errorManager.CreateErrorAction(context);
        }
    }
}