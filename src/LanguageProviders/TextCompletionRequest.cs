using System;

namespace Cheevly.LanguageProviders {
    public struct TextCompletionRequest {
        public string Text { get; set; }
        public int MaxTokens { get; set; }
        public decimal Temperature { get; set; }
        public List<string> Stops { get; set; }
    }
}