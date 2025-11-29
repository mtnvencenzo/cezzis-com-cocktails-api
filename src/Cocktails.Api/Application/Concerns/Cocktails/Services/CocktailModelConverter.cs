namespace Cocktails.Api.Application.Concerns.Cocktails.Services;

using global::Cocktails.Api.Application.Concerns.Cocktails.Models;
using global::Cocktails.Api.Domain.Aggregates.CocktailAggregate;
using global::Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using System.ComponentModel;

/// <summary>Converts a Cocktail domain model to a CocktailModel.</summary>
public class CocktailModelConverter : ICocktailModelConverter
{
    private readonly static TypeConverter glasswareConverter = TypeDescriptor.GetConverter(typeof(GlasswareTypeModel));
    private readonly static TypeConverter uomConverter = TypeDescriptor.GetConverter(typeof(UofMTypeModel));
    private readonly static TypeConverter ingredientTypeConverter = TypeDescriptor.GetConverter(typeof(IngredientTypeModel));
    private readonly static TypeConverter ingredientApplicationConverter = TypeDescriptor.GetConverter(typeof(IngredientApplicationModel));
    private readonly static TypeConverter preparationTypeConverter = TypeDescriptor.GetConverter(typeof(PreparationTypeModel));
    private readonly static TypeConverter requirementTypeConverter = TypeDescriptor.GetConverter(typeof(IngredientRequirementTypeModel));

    /// <summary>Converts a Cocktail domain model to a CocktailModel.</summary>
    public CocktailModel ToCocktailModel(Domain.Aggregates.CocktailAggregate.Cocktail item)
    {
        return item != null
            ? new CocktailModel
            (
                Content: item.Content?
                    .Replace("{{ingredients}}", item.GetIngredientsMarkDownDescription())
                    .Replace("{{instructions}}", item.GetInstructionsMarkDownDescription())
                    .Replace("{{iba}}", item.GetIbaDescription()),
                Glassware: [.. item.Glassware.Select(x => (GlasswareTypeModel)glasswareConverter.ConvertFrom(Enum.Parse<GlasswareType>(x, true)))],
                Id: item.Id,
                Serves: item.Serves,
                Tags: [.. item.Eras],
                SearchableTitles: [.. item.SearchableTitles],
                Title: item.Title,
                DescriptiveTitle: item.DescriptiveTitle,
                Description: !string.IsNullOrWhiteSpace(item.Description)
                    ? item.Description
                    : item.DescriptiveTitle,
                IsIba: item.IsIba,
                ModifiedOn: item.ModifiedOn,
                PublishedOn: item.PublishedOn,
                PrepTimeMinutes: item.PrepTimeMinutes,
                Rating: item.Rating != null
                    ? new CocktailRatingModel(item.Rating.OneStars, item.Rating.TwoStars, item.Rating.ThreeStars, item.Rating.FourStars, item.Rating.FiveStars, item.Rating.TotalStars, item.Rating.Rating, item.Rating.RatingCount)
                    : new CocktailRatingModel(0, 0, 0, 0, 0, 0, 0, 0),
                Instructions: [.. item.Instructions.OrderBy(x => x.Order).Select(x => new InstructionStepModel
                (
                    Display: x.DisplayValue,
                    Order: x.Order
                ))],
                MainImages: [.. item.Images.Where(x => x.Type == CocktailImageType.Main).Select(x => new CocktailImageModel
                (
                    Uri: x.Uri,
                    Width: x.Width,
                    Height: x.Height
                ))],
                SearchTiles: [.. item.Images.Where(x => x.Type == CocktailImageType.SearchTile).Select(x => new CocktailImageModel2
                (
                    Uri: x.Uri,
                    Width: x.Width,
                    Height: x.Height
                ))],
                Ingredients: [.. item.Ingredients.Select(x => new IngredientModel
                (
                    UoM: (UofMTypeModel)uomConverter.ConvertFrom(x.UoM),
                    Name: x.Name,
                    Preparation: (PreparationTypeModel)preparationTypeConverter.ConvertFrom(x.Preparation),
                    Requirement: (IngredientRequirementTypeModel)requirementTypeConverter.ConvertFrom(x.Requirement),
                    Suggestions: x.Suggestions?.ToString() ?? string.Empty,
                    Types: [.. x.Types.Select(x => (IngredientTypeModel)ingredientTypeConverter.ConvertFrom(Enum.Parse<IngredientType>(x, true)))],
                    Applications: [.. x.Applications.Select(x => (IngredientApplicationModel)ingredientApplicationConverter.ConvertFrom(Enum.Parse<IngredientApplication>(x, true)))],
                    Units: x.Units,
                    Display: x.GetDisplayValue()
                ))]
            ) : null;
    }
}