using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using DocumentImportLambda.Aws.Services;
using Microsoft.Extensions.Hosting.Internal;

LambdaHost host = LambdaHost
    .CreateBuilder(
#if DEBUG
        new HostingEnvironment { EnvironmentName = "Development" }
#endif
        )
    .Build(new StoneEagleLambdaConfigurationOptions()
    {
#if DEBUG
        AssemblyWithSecrets = typeof(Program).Assembly,
#endif
        ConfigureServices = (defaultImpl, serviceCollection, configurationRoot) => defaultImpl()
    });

// Change the function signature to use SQSEvent
Func<SQSEvent, ILambdaContext, Task> function = LambdaService.FunctionHandler;

// Continue with the Lambda runtime client setup
await LambdaBootstrapBuilder.Create(function, new DefaultLambdaJsonSerializer())
    .Build()
    .RunAsync()
    .ConfigureAwait(false);