using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cheevly.Extensions;
using Newtonsoft.Json;

namespace Cheevly.LanguageProviders.OpenAI {

    public class OpenAIClient : IDisposable, IsTextCompletionProvider {

        public OpenAIEngines Engine { get; set; }

        private const string _baseUrl = "https://api.openai.com/v1/";
        private readonly HttpClient _client;

        public OpenAIClient(string apiKey, OpenAIEngines engine = OpenAIEngines.TextDavinci003) {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            Engine = engine;
        }

        public void Dispose() {
            _client?.Dispose();
        }

        public async Task<string> GetEnginesAsync() {
            var response = await _client.GetAsync(_baseUrl + "engines");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IsTextCompletionResult> CompleteTextAsync(TextCompletionRequest request) {
            return await Completion(new OpenAICompletionQuery(Engine, request.Text) {
                Temperature = request.Temperature,
                MaxTokens = request.MaxTokens,
                Stops = request.Stops
            });
        }

        public async Task<OpenAICompletionResult> Completion(OpenAICompletionQuery query) {
            var engineName = GetEngineName(query.Engine);
            var url = _baseUrl + $"engines/{engineName}/completions";
            var queryTime = DateTime.UtcNow;

            var body = query.ToRequestBody();
            var response = await _client.PostAsJsonAsync(url, body, CancellationToken.None);
            var json = await response.Content.ReadAsStringAsync();
            var responseTime = DateTime.UtcNow;
            var result = JsonConvert.DeserializeObject<OpenAICompletionResponse>(json);
            var latency = (responseTime - queryTime).TotalMilliseconds;

            return new OpenAICompletionResult {
                Id = result.Id,
                QueryTime = queryTime.ToUnixTimestamp(),
                ResponseTime = responseTime.ToUnixTimestamp(),
                Latency = Convert.ToInt32(latency),
                Model = result.Model,
                Text = result.Choices[0].Text,
                FinishReason = result.Choices[0].FinishReason,
                Query = query
            };
        }

        public async Task<OpenAIEmbeddingResponse> Embedding(string query) {
            var response = await _client.PostAsJsonAsync(_baseUrl + "embeddings", new {
                input = query,
                model = "text-embedding-ada-002"
            }, CancellationToken.None);

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OpenAIEmbeddingResponse>(json);
        }

        public static string GetEngineName(OpenAIEngines engine) {
            return engine.ToString().SplitCamelCase('-').ToLower();
        }
    }
}