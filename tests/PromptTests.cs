using Cheevly.CodeExecution;
using Cheevly.Intent;
using Cheevly.Skills;

namespace Cheevly {

    [TestClass]
    public class PromptTests {

        [TestMethod]
        public void PromptExecutionPipeline_Execute() {
            var time = DateTime.Now;
            
            var configuration = new IntentRoutingConfiguration();
            configuration.Use(new TimeSkills(time)).Route("what time is it?", clock => clock.GetTime());

            var runner = new PromptExecutionRuntime(configuration.Mappings);
            var result = runner.Execute("clock.GetTime()");

            Assert.AreEqual(time, result);
        }
    }
}