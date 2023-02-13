using Cheevly.CodeExecution;
using Cheevly.LanguageProviders;
using Cheevly.Prompts;
using Cheevly.Prompts.Formatters;

namespace Cheevly.Intent {
    public class IntentDispatcher {
        private IntentRoutingConfiguration _configuration;
        private CSharpPromptFormatter _formatter = new();
        private IsTextCompletionProvider _provider;

        public IntentDispatcher(IntentRoutingConfiguration configuration, IsTextCompletionProvider provider) {
            _configuration = configuration;
            _provider = provider;
        }

        public async Task DispatchAsync(PromptContext context, Func<Task> next) {

            var samples = _configuration.Mappings
                .SelectMany(mapping => mapping.Routes)
                .Select(route => route.Sample)
                .ToArray();

            var prompt = _formatter.Format(context.Text, samples);

            var result = await _provider.CompleteTextAsync(new TextCompletionRequest {
                Text = prompt,
                MaxTokens = 128,
                Stops = new List<string> {
                    //"\n",
                    "\"\r",
                    "\" }"
                }
            });

            var runtime = new PromptExecutionRuntime(_configuration.Mappings);
            context.Response = runtime.Execute(result.Text);

            await next();
        }
    }
}
