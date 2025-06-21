namespace Cocktails.Api.Infrastructure.Unit.Tests;

using Asp.Versioning;
using Cezzi.Smtp;
using Cocktails.Api.Application.Behaviors.ExceptionHandling;
using Cocktails.Api.Infrastructure.Unit.Tests.Mocks;
using Cocktails.Api.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Moq;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

public abstract class ServiceTestBase : IDisposable
{
    protected readonly Mock<IHttpContextAccessor> httpContextAccessorMock;
    protected readonly Mock<ISmtpClientFactory> smtpClientFactoryMock;
    protected readonly Mock<ISmtpMailClient> smtpClientMock;
    protected readonly CancellationTokenSource cancellationTokenSource;
    protected MockHttpContext httpContext;
    private IServiceProvider serviceProvider;

    public ServiceTestBase()
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        this.smtpClientFactoryMock = new Mock<ISmtpClientFactory>();
        this.smtpClientMock = new Mock<ISmtpMailClient>();

        this.smtpClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(this.smtpClientMock.Object);

        this.httpContextAccessorMock
            .Setup(m => m.HttpContext)
            .Returns(() =>
            {
                this.httpContext ??= new MockHttpContext(
                    serviceProvider: this.serviceProvider,
                    cancellationToken: this.cancellationTokenSource.Token);

                return this.httpContext;
            });
    }

    public IServiceProvider SetupEnvironment(Action<IServiceCollection> servicePreprocessor = null, bool skipServiceReplacememt = false)
    {
        var basePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(ExceptionBehavior)).Location);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "unittest",
            ContentRootPath = basePath,
            ApplicationName = "Cocktails.Api.Unit.Tests",
            WebRootPath = basePath
        });

        var physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly());
        var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);
        builder.Services.AddSingleton<IFileProvider>(compositeProvider);

        var webhostEnvironment = new Mock<IWebHostEnvironment>();
        webhostEnvironment.Setup(m => m.ContentRootPath).Returns(basePath);
        webhostEnvironment.Setup(m => m.EnvironmentName).Returns("unittest");
        webhostEnvironment.Setup(x => x.WebRootFileProvider).Returns(physicalProvider);
        webhostEnvironment.Setup(x => x.ContentRootFileProvider).Returns(physicalProvider);
        builder.Services.AddSingleton(webhostEnvironment.Object);

#pragma warning disable CS0618 // Type or member is obsolete
        var hostingEnvMock = new Mock<IHostingEnvironment>();
        hostingEnvMock.Setup(m => m.ContentRootPath).Returns(basePath);
        hostingEnvMock.Setup(m => m.EnvironmentName).Returns("unittest");
        hostingEnvMock.Setup(x => x.WebRootFileProvider).Returns(physicalProvider);
        hostingEnvMock.Setup(x => x.ContentRootFileProvider).Returns(physicalProvider);
        builder.Services.AddSingleton(hostingEnvMock.Object);
#pragma warning restore CS0618 // Type or member is obsolete

        var listener = new DiagnosticListener("Microsoft.AspNetCore");
        builder.Services.AddSingleton(listener);
        builder.Services.AddSingleton<DiagnosticSource>(listener);

        void internalPreprocessor(IServiceCollection services)
        {
            services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            services.Replace(new ServiceDescriptor(typeof(ISmtpClientFactory), this.smtpClientFactoryMock.Object));
            servicePreprocessor?.Invoke(services);
        }

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
        if (skipServiceReplacememt == false)
        {
            internalPreprocessor(builder.Services);
        }

        var app = builder.Build();

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

        this.serviceProvider = app.Services;

        return this.serviceProvider;
    }

    public static string GuidString() => Guid.NewGuid().ToString();

    private static void Verify_NoOtherCalls() { }

    public void Dispose() => Verify_NoOtherCalls();
}
