using Asp.Versioning;
using Cocktails.Api.Application.Behaviors.ExceptionHandling;
using Cocktails.Api.StartupExtensions;
using Microsoft.AspNetCore.Diagnostics;
using Cocktails.Api.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults("cocktails-api");
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

// Initialize database if needed
if (app.Environment.IsEnvironment("local"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<StorageInitializer>().InitializeAsync();
    await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().InitializeAsync();
}

// Use cloud events to automatically unpack the message data
// app.UseCloudEvents();

app.UseApplicationEndpoints();
app.UseDefaultOpenApi();

// Not requiring the dev cert for open api locally
// Had issues with cert trust on ubuntu for some reason.
if (app.Environment.IsEnvironment("local"))
{
    app.UseWhen(context =>
    {
        return !context.Request.Path.Equals("/scalar/v1/openapi.json");
    }, appBuilder =>
    {
        appBuilder.UseHttpsRedirection();
    });
}
else
{
    app.UseHttpsRedirection();
}

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
            await ExceptionBehavior.OnException(context: context, ex: exceptionHandlerFeature.Error);
        }
    });
});

app.Run();

return 0;