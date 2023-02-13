using System;

namespace Cheevly.LanguageProviders {
    public interface IsTextCompletionProvider {
        Task<IsTextCompletionResult> CompleteTextAsync(TextCompletionRequest request);
    }
}