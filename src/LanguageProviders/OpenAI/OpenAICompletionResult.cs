namespace Cheevly.LanguageProviders.OpenAI {
    public class OpenAICompletionResult : IsTextCompletionResult {
        public OpenAICompletionQuery Query { get; set; }
        public string Id { get; set; }
        public long QueryTime { get; set; }
        public long ResponseTime { get; set; }
        public string Model { get; set; }
        public string Text { get; set; }
        public string FinishReason { get; set; }
        public int Latency { get; set; }

        public override string ToString() {
            return Text;
        }
    }
}