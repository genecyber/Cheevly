# Cheevly
##Introduction
Cheevly is an open-source virtual assistant platform that helps you work smarter and achieve more. Backed by large language models like OpenAI's GPT, Cheevly is able to quickly learn and perform tasks with the tools, processes and software that you use daily. This .NET Core library provides everything you need to configure, extend and deploy Cheevly for any environment.

## Why?
As large language models have advanced, the potential use-cases for them has grown beyond simple Q&A. It's now possible to harness them for more advanced task-oriented workflows. We're building the Cheevly platform to give developers the tools they need to quickly and easily integrate Cheevly into any environment.

## Features
 - *Lightweight & Modular:* The core represents a simple, clean processing pipeline that allows you to easily configure only the modules you need.
 - *Admin Portal:* A self-hosted web portal acts as an optional command-center to monitor or remotely control your agent(s).
 - *Integrate:* We've made it as simple as possible to connect Cheevly into your existing software and workflow.
 - *Skills:* Easily define new 'skills' for Cheevly by mapping English instructions to code

## Skills
We offer a variety of skills right out of the box that you can instantly activate on Cheevly
 - *Time:* Time-management and scheduling
 - *Files:* Windows file system
 - *Email:* Send and receive email through SMTP
 - *Video:* Record video and capture screenshots
 - *SMS:* Send and receive SMS text messages
 - *Input:* Mouse and keyboard
 - *Stable Diffusion:* Generate or edit images
 - *Code:* Execute or control .NET or Javascript code

##Cheevly Pro integration
Cheevly Pro enables you to 'teach by doing'. By narrating recorded videos of you performing tasks, Cheevly quickly learns to understand your language and the actions being performed.

##Roadmap

### Getting Started
```C#
var agent = new AgentBuilder()
    .UseOpenAI("API KEY")
    .UseIntentRouting()
    .Build();

Console.WriteLine(await agent.PromptAsync("what time is it?"));
```

#### Intent routing
```C#
var agent = new AgentBuilder()
    .UseOpenAI("API KEY")
    .UseIntentRouting(router => {

        router.Use(someObject)
            .Route("turn off the thing", someObject => someObject.Thing = false
        );
    })
    .Build();

await agent.PromptAsync("can you go turn the thing off");
Assert.AreEqual(someObject.Thing, false);
await agent.PromptAsync("sweet, now turn it back on");
Assert.AreEqual(someObject.Thing, true);
```