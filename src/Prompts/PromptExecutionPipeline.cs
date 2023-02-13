using System;

namespace Cheevly.Prompts {
    public class PromptExecutionPipeline {
        public delegate Task PipelineStep(PromptContext context, Func<Task> next);

        private List<PipelineStep> _steps = new();

        public void Use(PipelineStep step) {
            _steps.Add(step);
        }

        public Task HandlePromptAsync(PromptContext prompt) {
            if(_steps.Any()) {
                return NextPipelineStepAsync(prompt, _steps[0]);
            }

            return Task.CompletedTask;
        }

        private Task NextPipelineStepAsync(PromptContext prompt, PipelineStep step) {
            return step(prompt, () => {
                var next = _steps.IndexOf(step) + 1;

                if(next == _steps.Count)
                    return Task.CompletedTask;

                return NextPipelineStepAsync(prompt, _steps[next]);
            });
        }
    }
}