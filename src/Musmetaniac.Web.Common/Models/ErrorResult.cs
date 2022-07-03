using System;
using System.Net;
using Musmetaniac.Common.Exceptions;

namespace Musmetaniac.Web.Common.Models
{
    public class ErrorResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorResult()
        {
        }

        public ErrorResult(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ErrorResult(Exception exception)
        {
            var baseException = exception.GetBaseException();

            Message = baseException.Message;
            StatusCode = baseException is BusinessException ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError;
        }
    }
}
