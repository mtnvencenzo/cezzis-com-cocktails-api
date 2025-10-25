namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using MediatR;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using global::Cocktails.Api.Infrastructure;

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
        var any = (await cocktailRepository.Items.FirstOrDefaultAsync(cancellationToken)) != null;

        if (command.OnlyIfEmpty && any)
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