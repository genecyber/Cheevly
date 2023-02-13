namespace Cheevly.LanguageProviders.OpenAI
{
    public class OpenAIEmbeddingResponse
    {
        public string Object { get; set; }
        public List<EmbeddingItem> Data { get; set; }

        public class EmbeddingItem
        {
            public double[] Embedding { get; set; }
            public int Index { get; set; }
        }
    }
}