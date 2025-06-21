namespace Cocktails.Api.Application.Concerns.Cocktails.Commands;

using MediatR;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Cezzi.Applications.Extensions;
using global::Cocktails.Api.Infrastructure;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Microsoft.EntityFrameworkCore;
using global::Cocktails.Api.Infrastructure.Resources.Ingredients;

public record SeedIngredientsCommand(bool OnlyIfEmpty = false) : IRequest<bool>;

public class SeedIngredientsCommandHandler(
    IIngredientRepository ingredientRepository,
    IngredientsDataStore ingredientsDataStore,
    ILogger<SeedIngredientsCommandHandler> logger) : IRequestHandler<SeedIngredientsCommand, bool>
{
    private readonly static JsonSerializerOptions jsonSerializerOptions;

    static SeedIngredientsCommandHandler()
    {
        jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public async Task<bool> Handle(SeedIngredientsCommand command, CancellationToken cancellationToken)
    {
        var availableIngredients = ingredientsDataStore.Ingredients.ToList();
        var availableIds = availableIngredients.Select(x => x.Id).ToList();

        var allExisting = await ingredientRepository.Items.ToListAsync(cancellationToken);

        if (command.OnlyIfEmpty && allExisting.Count != 0)
        {
            return false;
        }

        var toDelete = allExisting.Where(a => !availableIds.Any(b => b == a.Id)).ToList();
        var hasChanges = false;

        foreach (var ingredient in availableIngredients)
        {
            var existing = allExisting.FirstOrDefault(x => x.Id == ingredient.Id);

            if (existing == null)
            {
                ingredientRepository.Add(ingredient);
                logger.LogInformation("Adding ingredient {IngredientId}", ingredient.Id);
                hasChanges = true;
            }
            else
            {
                if (existing.Hash != ingredient.RegenerateHash())
                {
                    logger.LogInformation("Updating ingredient {IngredientId}", ingredient.Id);
                    existing.MergeUpdate(ingredient);
                    hasChanges = true;
                }
            }
        }

        foreach (var delete in toDelete)
        {
            logger.LogInformation("Deleting ingredient {IngredientId}", delete.Id);
            ingredientRepository.Delete(delete);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await ingredientRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            ingredientRepository.ClearCache();
        }

        return true;
    }
}