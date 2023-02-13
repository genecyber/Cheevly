using Cheevly.Intent;
using Cheevly.Runtime;
using Cheevly.Skills;
using Cheevly.Tests;

namespace Cheevly {

    [TestClass]
    public class IntentTests {

        // Todo: mock agent

        [TestMethod]
        public async Task IntentRoutingConfiguration_RouteWithReturnValue() {

            var configuration = new IntentRoutingConfiguration();

            configuration.Use(new TimeSkills())
                .Route("what time is it?", clock => clock.GetTime());

            Assert.AreEqual(1, configuration.Mappings.Count);
            Assert.AreEqual("clock.GetTime()", configuration.Mappings[0].Routes[0].ToString());
        }

        [TestMethod]
        public async Task ShouldMapIntent_WithParameter() {

            var configuration = new IntentRoutingConfiguration();

            configuration.Use(new TimeSkills())
                .Route("set the clock to 10:57 pm", clock => clock.SetTime(DateTime.Parse("10:57 pm")));

            Assert.AreEqual(1, configuration.Mappings.Count);
            Assert.AreEqual("clock.SetTime(DateTime.Parse(\"10:57 pm\"))", configuration.Mappings.First().Routes.First().ToString());
        }

        [TestMethod]
        public async Task ShouldInvokeRoute_WithReturnValue() {

            var time = DateTime.Parse("1/1/2021 1:00 PM");

            var agent = new AgentBuilder()
                .UseOpenAI(UnitTestSettings.OpenAIKey)
                .UseIntentRouting(router => {

                    router.Use(new TimeSkills(time))
                        .Route("what time is it?", clock => clock.GetTime())
                        .Route("set the clock to 10:57 pm", clock => clock.SetTime(DateTime.Parse("10:57 pm"))
                    );
                }).Build();

            var result = await agent.PromptAsync("what's the currrent time");

            Assert.AreEqual(time, result);
        }

        [TestMethod]
        public async Task ShouldInvokeRoute_WithDualIntent() {

            var time = DateTime.Parse("1/1/2021 1:00 PM");

            var agent = new AgentBuilder()
                .UseOpenAI(UnitTestSettings.OpenAIKey)
                .UseIntentRouting(router => {

                    router.Use(new TimeSkills(time))
                        .Route("what time is it?", clock => clock.GetTime())
                        .Route("set the clock to 10:57 pm", clock => clock.SetTime(DateTime.Parse("10:57 pm")))
                        .Route("add an hour", clock => clock.AddTime(new TimeSpan(1, 0, 0))
                    );
                }).Build();


            var test = await agent.PromptAsync("multiply 3*3 as minutes to the current time");
            var result = await agent.PromptAsync("what's the current time");

            Assert.IsTrue(result is DateTime);
            Assert.AreEqual("1/1/2021 1:09:00 PM", result.ToString());
        }
    }
}