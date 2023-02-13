using Cheevly.Extensions;
using Cheevly.Intent;
using Cheevly.LanguageProviders.OpenAI;
using Cheevly.Prompts;
using Cheevly.Skills;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cheevly.Runtime {
    public class AgentBuilder {
        private HostBuilder _builder = new HostBuilder();
        private PromptExecutionPipeline _pipeline = new();

        public AgentBuilder() {
            Use(_pipeline);
        }

        public AgentBuilder UseOpenAI(string apiKey) {
            Use(new OpenAIClient(apiKey));
            return this;
        }

        public AgentBuilder UseIntentRouting(Action<IntentRoutingConfiguration> configure) {

            Use<IntentDispatcher>(ServiceLifetime.Scoped);

            Use<IntentRoutingConfiguration>((provider) => {
                var configuration = new IntentRoutingConfiguration();
                configure(configuration);

                // Todo: move into the skill system
                configuration.Use(new TimeSkills())
                    .Route("what's the current time?", clock => clock.GetTime());

                return configuration;
            });

            _pipeline.Use((context, next) => {
                var dispatcher = context.Services.GetService<IntentDispatcher>();
                return dispatcher!.DispatchAsync(context, next);
            });

            return this;
        }

        public CheevlyAgent Build() {
            return new CheevlyAgent(_builder.Build(), _pipeline);
        }

        public AgentBuilder Use(object service) {
            _builder.ConfigureServices((context, services) => {
                var descriptor = new ServiceDescriptor(service.GetType(), service);
                services.Add(descriptor);

                foreach (var each in GetTypes(service.GetType()))
                    services.Add(new ServiceDescriptor(each, service));
            });

            return this;
        }

        public AgentBuilder Use<T>(Func<IServiceProvider, object> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton) {
            _builder.ConfigureServices((context, services) => {
                services.Add(new ServiceDescriptor(typeof(T), factory, lifetime));

                foreach (var each in GetTypes(typeof(T)))
                    services.Add(new ServiceDescriptor(each, provider => provider.GetRequiredService(typeof(T)), lifetime));
            });

            return this;
        }

        public AgentBuilder Use<T>(ServiceLifetime lifetime = ServiceLifetime.Singleton) {
            _builder.ConfigureServices((context, services) => {
                services.Add(new ServiceDescriptor(typeof(T), typeof(T), lifetime));

                foreach (var each in GetTypes(typeof(T)))
                    services.Add(new ServiceDescriptor(each, provider => provider.GetRequiredService(typeof(T)), lifetime));
            });

            return this;
        }

        private static IEnumerable<Type> GetTypes(Type type) {
            return type.GetBaseTypes().Where(each => each.Namespace != null && !each.Namespace.StartsWith("System"));
        }
    }
}