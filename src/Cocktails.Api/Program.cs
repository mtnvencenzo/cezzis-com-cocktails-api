using Asp.Versioning;
using Cocktails.Api.Application.Behaviors.ExceptionHandling;
using Cocktails.Api.Domain.Config;
using Cocktails.Api.StartupExtensions;
using Cocktails.Api.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System.Diagnostics;
using MediatR;
using Cocktails.Api.Application.Concerns.Cocktails.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddApplicationServices();

var apiVersioningBuilder = builder.Services.AddApiVersioning((o) =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
});

builder.AddDefaultOpenApi(apiVersioningBuilder);

// -------------
// build the app
// -------------
var app = builder.Build();

#if DEBUG
Debugger.Launch();
#endif

// Initialize database if needed
if (app.Environment.IsEnvironment("local"))
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();

    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    await mediator.Send(new SeedIngredientsCommand(OnlyIfEmpty: true));
    await mediator.Send(new SeedCocktailsCommand(OnlyIfEmpty: true));
}

// Use cloud events to automatically unpack the message data
// app.UseCloudEvents();

app.UseApplicationEndpoints();
app.UseDefaultOpenApi();

app.UseHttpsRedirection();
app.UseCors("origin-policy");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseExceptionHandler((builder) =>
{
    builder.Run(async (context) =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature?.Error != null)
        {
            await ExceptionBehavior.OnException(context: context, ex: exceptionHandlerFeature.Error).ConfigureAwait(false);
        }
    });
});

app.Run();

return 0;