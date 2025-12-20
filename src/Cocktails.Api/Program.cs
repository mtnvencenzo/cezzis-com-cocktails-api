using Asp.Versioning;
using Cocktails.Api.Application.Behaviors.ExceptionHandling;
using Cocktails.Api.StartupExtensions;
using Microsoft.AspNetCore.Diagnostics;

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

// Use cloud events to automatically unpack the message data
// app.UseCloudEvents(); // test 

app.UseApplicationEndpoints();
app.UseDefaultOpenApi();
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

if (Environment.GetEnvironmentVariable("FORCE_HTTPS") != "false")
{
    app.UseHttpsRedirection();
}

await app.UseDaprJobs();

app.Run();

return 0;