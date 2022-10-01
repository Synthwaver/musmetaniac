using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Musmetaniac.Common.Extensions;
using Musmetaniac.Services.Exceptions;
using Newtonsoft.Json.Linq;

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
            return (await GetJsonAsync(apiMethod, parameters)).FromJson<T>()!;
        }

        protected async Task<string> GetJsonAsync(string apiMethod, IReadOnlyCollection<KeyValuePair<string, string>> parameters)
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

            ThrowIfResponseHasError(response, responseContent);

            return responseContent;
        }

        private static void ThrowIfResponseHasError(HttpResponseMessage responseMessage, string responseContent)
        {
            var jToken = JToken.Parse(responseContent);

            if (jToken.Value<int?>("error").HasValue)
                throw new LastFmApiRequestException(jToken.Value<string>("message"));

            if (!responseMessage.IsSuccessStatusCode)
                throw new LastFmApiRequestException(responseMessage.ReasonPhrase);
        }
    }
}
