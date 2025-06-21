namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using MediatR;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Cezzi.Applications.Extensions;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Infrastructure.Resources;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using global::Cocktails.Api.Infrastructure.Resources.TraditionalCocktails;
using global::Cocktails.Api.Infrastructure.Resources.ModernCocktails;
using global::Cocktails.Api.Infrastructure;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using global::Cocktails.Api.Infrastructure.Resources.Ingredients;

public record SeedCocktailsCommand(bool OnlyIfEmpty = false) : IRequest<bool>;

public class SeedCocktailsCommandHandler(
    ICocktailRepository cocktailRepository,
    CocktailDataStore cocktailsDataStore,
    ILogger<SeedCocktailsCommandHandler> logger) : IRequestHandler<SeedCocktailsCommand, bool>
{
    private readonly static string bomMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
    private readonly static JsonSerializerOptions jsonSerializerOptions;

    static SeedCocktailsCommandHandler()
    {
        jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public async Task<bool> Handle(SeedCocktailsCommand command, CancellationToken cancellationToken)
    {
        var availableCocktails = cocktailsDataStore.Cocktails;
        var hasChanges = false;
        var count = await cocktailRepository.Items.CountAsync(cancellationToken);

        if (command.OnlyIfEmpty && count > 0)
        {
            return false;
        }

        foreach (var cocktail in availableCocktails)
        {
            var existing = await cocktailRepository.Items.FirstOrDefaultAsync(x => x.Id == cocktail.Id, cancellationToken);

            if (existing == null)
            {
                cocktailRepository.Add(cocktail);

                logger.LogInformation("Adding cocktail {CocktailId}", cocktail.Id);
                hasChanges = true;
            }
            else
            {
                if (existing.Hash != cocktail.RegenerateHash())
                {
                    logger.LogInformation("Updating cocktail {CocktailId}", cocktail.Id);
                    existing.MergeUpdate(cocktail);
                    hasChanges = true;
                }
            }
        }

        if (hasChanges)
        {
            await cocktailRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            cocktailRepository.ClearCache();
        }

        return true;
    }
}