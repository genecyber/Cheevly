using Cheevly.Prompts;

namespace Cheevly.Intent {
    public class IntentRoute {
        public PromptSample Sample { get; set; }
        public List<object> Parameters { get; set; } = new();
        public string VariableName { get; set; }

        public override string ToString() {
            return Sample.Output;
        }
    }
}