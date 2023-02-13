using System;

namespace Cheevly.Prompts {
    public class PromptContext {
        public string Text { get; set; }
        public object Response { get; set; }
        public IServiceProvider Services { get; set; }
    }
}