namespace Cocktails.Api.Application.IntegrationEvents;

using Cezzi.Applications;
using Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using Cocktails.Api.Domain.Services;
using FluentValidation;
using MediatR;
using System;
using System.Text.Json.Serialization;

public class CocktailRatingEvent(string ownedAccountId, string ownedAccountSubjectId, string cocktailId, int stars) : IIntegrationEvent, IRequest<bool>
{
    [JsonInclude]
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonInclude]
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;

    [JsonInclude]
    public string OwnedAccountId { get; } = ownedAccountId ?? throw new ArgumentNullException(nameof(ownedAccountId));

    [JsonInclude]
    public string OwnedAccountSubjectId { get; } = ownedAccountSubjectId ?? throw new ArgumentNullException(nameof(ownedAccountSubjectId));

    [JsonInclude]
    public string CocktailId { get; } = cocktailId ?? throw new ArgumentNullException(nameof(cocktailId));

    [JsonInclude]
    public int Stars { get; } = stars;

    [JsonInclude]
    public bool DecrementRating { get; set; } = false;
}

public class CocktailRatingEventHandler(ICocktailRepository cocktailRepository) : IRequestHandler<CocktailRatingEvent, bool>
{
    public async Task<bool> Handle(CocktailRatingEvent command, CancellationToken cancellationToken)
    {
        var cocktail = await cocktailRepository.GetAsync(command.CocktailId, cancellationToken: cancellationToken);
        Guard.NotNull(cocktail);

        if (command.DecrementRating)
        {
            cocktail.DecrementRating(command.Stars);
        }
        else
        {
            cocktail.IncrementRating(command.Stars);
        }

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