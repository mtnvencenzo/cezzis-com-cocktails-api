namespace Cocktails.Api.Application.IntegrationEvents;

using Cezzi.Applications;
using Cezzi.Applications.Extensions;
using Cocktails.Api.Apis.Integrations;
using Cocktails.Api.Application.Exceptions;
using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Services;
using Cocktails.Api.Infrastructure.Repositories;
using Cocktails.Api.Infrastructure.Services;
using Cocktails.Common.Emails;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Text.Json.Serialization;

public class CocktailRatingEvent(string ownedAccountId, string ownedAccountSubjectId, string cocktailId, int stars) : IIntegrationEvent, IRequest<bool>
{
    [JsonInclude]
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

    [JsonInclude]
    public string OwnedAccountId { get; } = ownedAccountId ?? throw new ArgumentNullException(nameof(ownedAccountId));

    [JsonInclude]
    public string OwnedAccountSubjectId { get; } = ownedAccountSubjectId ?? throw new ArgumentNullException(nameof(ownedAccountSubjectId));

    [JsonInclude]
    public string CocktailId { get; } = cocktailId ?? throw new ArgumentNullException(nameof(cocktailId));

    [JsonInclude]
    public int Stars { get; } = stars;
}

public class CocktailRatingEventHandler(ICocktailRepository cocktailRepository) : IRequestHandler<CocktailRatingEvent, bool>
{
    public async Task<bool> Handle(CocktailRatingEvent command, CancellationToken cancellationToken)
    {
        var cocktail = await cocktailRepository.GetAsync(command.CocktailId, cancellationToken: cancellationToken);
        Guard.NotNull(cocktail);

        cocktail.IncrementRating(command.Stars);
        cocktailRepository.Update(cocktail);

        await cocktailRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        cocktailRepository.UpdateCache(cocktail);

        return true;
    }
}

public class CocktailRatingEventValidator : AbstractValidator<CocktailRatingEvent>, IValidator<CocktailRatingEvent>
{
    public CocktailRatingEventValidator()
    {
        this.RuleLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(x => x.OwnedAccountId).NotEmpty().MinimumLength(Guid.NewGuid().ToString().Length);
        this.RuleFor(x => x.OwnedAccountSubjectId).NotEmpty().MinimumLength(Guid.NewGuid().ToString().Length);
        this.RuleFor(x => x.CocktailId).NotEmpty();
        this.RuleFor(x => x.Stars).InclusiveBetween(1, 5);
    }
}