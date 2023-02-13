using Microsoft.Extensions.DependencyInjection;
using Cheevly.Prompts;
using Microsoft.Extensions.Hosting;

namespace Cheevly.Runtime {
    public class CheevlyAgent {

        // Todo: move this to an isolated dependency
        private readonly IHost _host;
        private readonly PromptExecutionPipeline _pipeline;

        public CheevlyAgent(IHost host, PromptExecutionPipeline pipeline) {
            _host = host;
            _pipeline = pipeline;
        }

        public async Task<object> PromptAsync(string prompt) {

            using var scope = _host.Services.CreateScope();

            var context = new PromptContext {
                Services = scope.ServiceProvider,
                Text = prompt
            };

            await _pipeline.HandlePromptAsync(context);

            // Todo: add the output sink
            if(context.Response != null) {
                return context.Response;
            }

            return string.Empty;
        }
    }
}