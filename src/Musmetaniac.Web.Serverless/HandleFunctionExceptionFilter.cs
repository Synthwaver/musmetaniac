using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Web.Common.Models;

namespace Musmetaniac.Web.Serverless
{
    public class HandleFunctionExceptionFilter : IFunctionExceptionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HandleFunctionExceptionFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            var errorResult = new ErrorResult(exceptionContext.Exception);

            if (errorResult.StatusCode == HttpStatusCode.InternalServerError)
                errorResult.Message = "An error has occurred. Please try again later.";

            var response = _httpContextAccessor.HttpContext.Response;
            var responseBody = errorResult.ToJson();

            response.StatusCode = (int)errorResult.StatusCode;
            response.ContentType = MediaTypeNames.Application.Json;
            response.ContentLength = responseBody.Length;

            return response.WriteAsync(responseBody, cancellationToken);
        }
    }
}
