namespace Cocktails.Api.StartupExtensions;

using Cocktails.Api.Domain.Config;
using Microsoft.Extensions.Options;
using Dapr.Jobs;
using Dapr.Jobs.Extensions;
using Dapr.Jobs.Models;
using Cocktails.Api.Application.Behaviors.DaprAppTokenAuthorization;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Polly.Retry;

internal static class DaprExtensions
{
    internal static IServiceCollection AddDaprClient(this IServiceCollection services)
    {
        services.AddTransient<DaprAppTokenRequirementHandler>();
        services.AddTransient<IAuthorizationHandler, DaprAppTokenRequirementHandler>();

        var authorizationBuilder = services.AddAuthorizationBuilder()
            .AddPolicy(DaprAppTokenRequirement.PolicyName, (o) =>
            {
                o.AddRequirements(new DaprAppTokenRequirement());
            });

        // Register the dapr client
        services.AddDaprClient((sp, builder) =>
        {
            var opts = sp.GetRequiredService<IOptions<DaprConfig>>().Value;
            var jsonOptions = sp.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value;

            builder.UseJsonSerializationOptions(jsonOptions.JsonSerializerOptions);

            if (!string.IsNullOrWhiteSpace(opts.DaprAppToken))
            {
                builder.UseDaprApiToken(opts.DaprAppToken);
            }

            if (!string.IsNullOrWhiteSpace(opts.HttpEndpoint))
            {
                builder.UseHttpEndpoint(opts.HttpEndpoint);
            }

            if (!string.IsNullOrWhiteSpace(opts.GrpcEndpoint))
            {
                builder.UseGrpcEndpoint(opts.GrpcEndpoint);
            }
        });

        // Register the dapr jobs client
        // Unfortunately there is no generic extension method to register custom dapr clients
        services.AddDaprJobsClient((sp, builder) =>
        {
            var opts = sp.GetRequiredService<IOptions<DaprConfig>>().Value;

            if (!string.IsNullOrWhiteSpace(opts.DaprAppToken))
            {
                builder.UseDaprApiToken(opts.DaprAppToken);
            }

            if (!string.IsNullOrWhiteSpace(opts.HttpEndpoint))
            {
                builder.UseHttpEndpoint(opts.HttpEndpoint);
            }

            if (!string.IsNullOrWhiteSpace(opts.GrpcEndpoint))
            {
                builder.UseGrpcEndpoint(opts.GrpcEndpoint);
            }
        });

        return services;
    }

#pragma warning disable DAPR_JOBS // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal static IApplicationBuilder UseDaprJobs(this IApplicationBuilder app)
    {
        var daprConfig = app.ApplicationServices.GetRequiredService<IOptions<DaprConfig>>().Value;

        if (daprConfig.InitJobEnabled)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() =>
            {
                var jobClient = app.ApplicationServices.GetRequiredService<DaprJobsClient>();
                var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(DaprExtensions));

                _ = ScheduleInitJobAsync(jobClient, logger);
            });
        }

        return app;
    }

    private static async Task ScheduleInitJobAsync(DaprJobsClient jobClient, ILogger logger)
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(20),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Failed to schedule initialize-app job. Retry attempt {AttemptNumber} after {RetryDelay}.",
                        args.AttemptNumber,
                        args.RetryDelay);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        try
        {
            await pipeline.ExecuteAsync(async ct =>
            {
                await jobClient.ScheduleJobAsync(
                    jobName: "initialize-app",
                    schedule: DaprJobSchedule.FromDateTime(DateTimeOffset.UtcNow.AddSeconds(10)),
                    startingFrom: DateTimeOffset.UtcNow,
                    repeats: 1,
                    overwrite: true,
                    cancellationToken: ct);
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to schedule initialize-app job after all retry attempts.");
        }
    }
#pragma warning restore DAPR_JOBS // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
