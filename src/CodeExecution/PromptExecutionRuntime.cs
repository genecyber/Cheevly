using Cheevly.Intent;
using Cheevly.Skills;

namespace Cheevly.CodeExecution {
    public class PromptExecutionRuntime {
        private List<IntentMapping> _mappings;

        public PromptExecutionRuntime(List<IntentMapping> mappings) {
            _mappings = mappings;
        }

        public object Execute(string code) {
            if (string.IsNullOrEmpty(code))
                return null;

            return GetRunner().InvokeMethod(FormatCode(code), GetParameters());
        }

        private string FormatCode(string code) {
            code = code.Trim();

            if (!code.EndsWith(";") && !code.EndsWith("}"))
                code += ";";

            return code;
        }

        private CSharpExecutionContext GetRunner() {
            return CSharpExecutionContext.WithTypeImports(typeof(TimeSkills));
        }

        private MethodParameterBinding[] GetParameters() {
            return _mappings.SelectMany(each => each.Routes.Select(route => new MethodParameterBinding {
                Name = route.VariableName,
                Value = each.Host
            }))
            .DistinctBy(each => each.Name)
            .ToArray();
        }
    }
}
