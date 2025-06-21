namespace Cocktails.Api.Application.IntegrationEvents;

using Cocktails.Api.Application.Exceptions;
using Cocktails.Api.Domain.Services;
using Cocktails.Api.Infrastructure.Services;
using Cocktails.Common.Emails;
using FluentValidation;
using MediatR;
using System;
using System.Text.Json.Serialization;

public class CocktailRecommendationEmailEvent : EmailMessage, IIntegrationEvent, IRequest<bool>
{
    public CocktailRecommendationEmailEvent()
    {
        this.Id = Guid.NewGuid().ToString();
        this.CreationDate = DateTimeOffset.UtcNow;
    }

    [JsonInclude]
    public string Id { get; set; }

    [JsonInclude]
    public DateTimeOffset CreationDate { get; set; }
}

public class CocktailRecommendationEmailEventHandler(
    IZohoEmailClient zohoEmailClient,
    ILogger<CocktailRecommendationEmailEventHandler> logger) : IRequestHandler<CocktailRecommendationEmailEvent, bool>
{
    public async Task<bool> Handle(CocktailRecommendationEmailEvent command, CancellationToken cancellationToken)
    {
        try
        {
            await zohoEmailClient.SendEmail(command, cancellationToken);
            return true;
        }
        catch (Exception ex) when (ex is not CocktailsApiValidationException)
        {
            logger.LogError(ex, "Failed to send zoho email.");
            throw;
        }
    }
}

public class CocktailRecommendationEmailEventValidator : AbstractValidator<CocktailRecommendationEmailEvent>, IValidator<CocktailRecommendationEmailEvent>
{
    public CocktailRecommendationEmailEventValidator()
    {
        this.RuleLevelCascadeMode = CascadeMode.Stop;

        // At least a body or subject must be specified
        this.When(command => string.IsNullOrWhiteSpace($"{command.Body}{command.Subject}"), () =>
        {
            this.RuleFor(command => command.Body).NotEmpty().WithMessage($"An email {nameof(CocktailRecommendationEmailEvent.Body)} or {nameof(CocktailRecommendationEmailEvent.Subject)} is required");
        });

        // A from address must be specified
        this.RuleFor(command => command.From)
            .NotNull()
            .WithMessage($"{nameof(CocktailRecommendationEmailEvent.From)} is required")
            .DependentRules(() =>
            {
                this.RuleFor(command => command.From.Address)
                    .NotEmpty().WithMessage($"{nameof(CocktailRecommendationEmailEvent.From)} {nameof(CocktailRecommendationEmailEvent.From.Address)} must not be empty")
                    .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage($"{nameof(CocktailRecommendationEmailEvent.From)} {nameof(CocktailRecommendationEmailEvent.From.Address)} is required and must be a valid email address");
            });

        // At least one to address must be specifiied and any specified should all value valid addresses
        this.RuleFor(command => command.To)
            .NotNull().WithMessage($"{nameof(CocktailRecommendationEmailEvent.To)} is required")
            .NotEmpty().WithMessage($"{nameof(CocktailRecommendationEmailEvent.To)} is required")
            .DependentRules(() =>
            {
                this.RuleForEach(command => command.To)
                    .Cascade(CascadeMode.Stop)
                    .ChildRules(to =>
                    {
                        to.RuleFor(x => x.Address)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty().WithMessage($"{nameof(CocktailRecommendationEmailEvent.To)} {nameof(EmailAddress.Address)} is required and must be a valid email address")
                            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage($"{nameof(CocktailRecommendationEmailEvent.To)} {nameof(EmailAddress.Address)} is required and must be a valid email address");
                    });
            });

        // if replyto is specified then the address must be a valid address
        this.When(command => command.ReplyTo != null, () =>
        {
            this.RuleFor(command => command.ReplyTo.Address)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage($"{nameof(CocktailRecommendationEmailEvent.ReplyTo)} {nameof(CocktailRecommendationEmailEvent.ReplyTo.Address)} must not be empty")
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage($"{nameof(CocktailRecommendationEmailEvent.ReplyTo)} {nameof(EmailAddress.Address)} must be a valid email address");
        });

        // if Cc is specified then each Cc must have a valid address
        this.When(command => command.Cc != null && command.Cc.Count > 0, () =>
        {
            this.RuleForEach(command => command.Cc)
                .Cascade(CascadeMode.Stop)
                .ChildRules(cc =>
                {
                    cc.RuleFor(x => x.Address)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty().WithMessage($"{nameof(CocktailRecommendationEmailEvent.Cc)} {nameof(EmailAddress.Address)} must be a valid email address")
                        .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage($"{nameof(CocktailRecommendationEmailEvent.Cc)} {nameof(EmailAddress.Address)} must be a valid email address");
                });
        });

        // if Bcc is specified then each Bcc must have a valid address
        this.When(command => command.Bcc != null && command.Bcc.Count > 0, () =>
        {
            this.RuleForEach(command => command.Bcc)
                .Cascade(CascadeMode.Stop)
                .ChildRules(bcc =>
                {
                    bcc.RuleFor(x => x.Address)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty().WithMessage($"{nameof(CocktailRecommendationEmailEvent.Bcc)} {nameof(EmailAddress.Address)} must be a valid email address")
                        .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage($"{nameof(CocktailRecommendationEmailEvent.Bcc)} {nameof(EmailAddress.Address)} must be a valid email address");
                });
        });
    }
}