using System;

namespace Cheevly.Prompts.Formatters {
    public interface IsPromptFormatter {
        string Format(string prompt, params PromptSample[] samples);
    }
}