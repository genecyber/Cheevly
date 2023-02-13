using System.Linq;
using System.Linq.Expressions;
using Cheevly.Prompts;
using ExpressionToCodeLib;

namespace Cheevly.Intent {

    public class IntentMapping {
        public object Host { get; set; }
        public List<IntentRoute> Routes { get; set; } = new();
    }

    public class IntentMapping<T> : IntentMapping {

        public IntentMapping(object host) {
            Host = host;
        }

        public IntentMapping<T> Route(string prompt, Expression<Action<T>> expression) {
            return Route<Action<T>>(new PromptSample {
                Input = prompt
            }, expression);
        }

        public IntentMapping<T> Route(string prompt, Expression<Func<T, object>> expression) {
            return Route<Func<T, object>>(new PromptSample {
                Input = prompt
            }, expression);
        }

        public IntentMapping<T> Route(PromptSample sample, Expression<Action<T>> expression) {
            return Route<Action<T>>(sample, expression);
        }

        public IntentMapping<T> Route(PromptSample sample, Expression<Func<T, object>> expression) {
            return Route<Func<T, object>>(sample, expression);
        }

        private IntentMapping<T> Route<X>(PromptSample sample, Expression<X> expression) where X : MulticastDelegate {
            var expressionSegments = ExpressionToCode.ToCode(expression).Split("=> ");
            var variableName = expressionSegments[0];
            sample.Output = string.Join(string.Empty, expressionSegments[1..]);

            Routes.Add(new IntentRoute {
                Sample = sample,
                VariableName = variableName
            });

            return this;
        }
    }
}