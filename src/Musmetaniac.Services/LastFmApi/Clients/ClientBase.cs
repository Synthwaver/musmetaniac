using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Services.Exceptions;

namespace Musmetaniac.Services.LastFmApi.Clients
{
    public class ClientBase
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        protected ClientBase(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        protected async Task<T> GetAsync<T>(string apiMethod, IReadOnlyCollection<KeyValuePair<string, string>> parameters) where T : class
        {
            parameters = new Dictionary<string, string>
            {
                ["api_key"] = _apiKey,
                ["method"] = apiMethod,
                ["format"] = "json",
            }.Concat(parameters).ToArray();

            var query = new QueryBuilder(parameters).ToQueryString().ToUriComponent();

            var response = await _httpClient.GetAsync(query);
            var responseContent = await response.Content.ReadAsStringAsync();

            HandleErrors(response, responseContent);

            return responseContent.FromJson<T>()!;
        }

        private static void HandleErrors(HttpResponseMessage responseMessage, string? responseContent)
        {
            var errorContent = responseContent.FromJson<ErrorResponseModel>();
            if (errorContent?.Error != null)
                throw new LastFmApiRequestException(errorContent.Message);

            if (!responseMessage.IsSuccessStatusCode)
                throw new LastFmApiRequestException(responseMessage.ReasonPhrase);
        }

        protected class ErrorResponseModel
        {
            public int? Error { get; set; }
            public string? Message { get; set; }
        }
    }
}
