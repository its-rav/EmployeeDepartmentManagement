using BusinessTier.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq.Dynamic.Core.Exceptions;
using System.Net;

namespace BusinessTier.Handlers
{
    public class ErrorHandlingFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ParseException)
            {
                context.Result = new ObjectResult(new ErrorResponse(HttpStatusCode.BadRequest.ToString(), context.Exception.ToString()))
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };
                context.ExceptionHandled = true;
                return;
            }
#if DEBUG
            context.Result = new ObjectResult(new ErrorResponse(HttpStatusCode.InternalServerError.ToString(), context.Exception.ToString()))
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
            context.ExceptionHandled = true;
#else
            context.Result = new ObjectResult(new ErrorResponse((int)HttpStatusCode.InternalServerError, "Opps, something went wrong!"))
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
            context.ExceptionHandled = true;
#endif

        }
    }
}
