using System;
using System.Text;

namespace Cheevly.Prompts.Formatters {
    public class CSharpPromptFormatter : IsPromptFormatter {
        public virtual string Format(string prompt, params PromptSample[] samples) {
            var builder = new StringBuilder();

            foreach (var sample in samples) {
                builder.AppendLine("// " + sample.Input);
                builder.AppendLine(sample.Output + Environment.NewLine);
            }

            builder.AppendLine(Environment.NewLine + "// " + prompt + Environment.NewLine);

            return builder.ToString();
        }
    }
}