namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Security.Recaptcha;
using FluentValidation;
using global::Cocktails.Api.Application.Behaviors;
using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Application.Concerns.RecaptchaVerification;
using global::Cocktails.Api.Application.IntegrationEvents;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using global::Cocktails.Api.Domain;
using global::Cocktails.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using global::Cocktails.Common.Emails;
using FluentValidation.Results;
using global::Cocktails.Api.Application.Concerns.RecaptchaVerification.Commands;
using global::Cocktails.Api.Application.Utilities;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;

public record CocktailRecommendationCommand
(
    CocktailRecommendationModel Recommendation,
    string VerificationCode
) : IRequest<bool>;

public class CocktailRecommendationCommandHandler(
    IEventBus eventBus,
    IOptions<EmailHandlingConfig> emailHandlingConfig,
    IOptions<PubSubConfig> pubSubConfig,
    ILogger<CocktailRecommendationCommandHandler> logger) : IRequestHandler<CocktailRecommendationCommand, bool>
{
    private readonly IEventBus eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    private readonly PubSubConfig pubSubConfig = pubSubConfig?.Value ?? throw new ArgumentNullException(nameof(pubSubConfig));
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<bool> Handle(CocktailRecommendationCommand command, CancellationToken cancellationToken)
    {
        if (this.pubSubConfig.EmailPublisher.SkipPublish)
        {
            this.logger.LogWarning("Skip publish enabled, not going to publish email message.");
            return true;
        }

        var cocktailRecommendationEvent = new CocktailRecommendationEmailEvent
        {
            Subject = "New Cocktail Recommendation for Cezzis.Com",
            CorrelationId = Guid.NewGuid().ToString(),
            To = [
                new EmailAddress
                {
                    DisplayName = emailHandlingConfig.Value.DefaultToName,
                    Address = emailHandlingConfig.Value.DefaultToAddress
                }
            ],
            From = new EmailAddress
            {
                DisplayName = emailHandlingConfig.Value.DefaultFromName,
                Address = emailHandlingConfig.Value.DefaultFromAddress
            },
            Body = @$"
                    <html>
                        <head>
                            <style>
                                body {{
                                    font-family: Arial, sans-serif;
                                }}
                            </style>
                        </head>
                        <body>
                            <h1>New Cocktail Recommendation</h1>
                            <p>
                                <strong>Recommendation:</strong><br/>{command.Recommendation?.GetRecommendation()}
                            </p>
                        </body>
                    </html>
                "
        };

        try
        {
            await this.eventBus.PublishAsync(
                @event: cocktailRecommendationEvent,
                messageLabel: "email-svc",
                contentType: "application/json",
                configName: this.pubSubConfig.EmailPublisher.DaprBuildingBlock,
                topicName: this.pubSubConfig.EmailPublisher.TopicName,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            var rawMessage = EventSerializer.ToJsonString(cocktailRecommendationEvent);

            using var messageScope = this.logger.BeginScope(new Dictionary<string, object>
            {
                { Monikers.App.ObjectGraph, rawMessage }
            });

            this.logger.LogCritical(ex, "Failed to send email message to topic");

            return false;
        }
    }
}

public class CocktailRecommendationCommandValidator : AbstractValidator<CocktailRecommendationCommand>, IAsyncValidator
{
    private readonly IMediator mediator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CocktailRecommendationCommandValidator(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(command => command.Recommendation).NotNull().WithMessage("Cocktail recommendation is required");
        this.RuleFor(command => command.Recommendation.Name).NotEmpty().WithMessage("Cocktail name is required");
        this.RuleFor(command => command.Recommendation.Directions).NotEmpty().WithMessage("Directions are required");
        this.RuleFor(command => command.Recommendation.Ingredients).NotEmpty().WithMessage("Ingredients are required");
        this.RuleFor(command => command.VerificationCode).NotEmpty().WithMessage("Recaptcha code required");
        this.RuleFor(command => command).CustomAsync(this.BeAValidRecaptchaToken);
    }

    private async Task BeAValidRecaptchaToken(CocktailRecommendationCommand command, ValidationContext<CocktailRecommendationCommand> validation, CancellationToken cancellationToken = default)
    {
        var result = await this.mediator.Send(new ValidateRecaptchaCommand(command.VerificationCode, this.httpContextAccessor.HttpContext.GetIPAddress()), cancellationToken);

        if (result?.VerificationStatus != RecaptchaVerificationStatus.Success)
        {
            var error = result?.ReturnCodes?.FirstOrDefault()?.Message ?? "Recaptcha verification failed";

            validation.AddFailure(new ValidationFailure(nameof(command.VerificationCode), error));
        }
    }
}