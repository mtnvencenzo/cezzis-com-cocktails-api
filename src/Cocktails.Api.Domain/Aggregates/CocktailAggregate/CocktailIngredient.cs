namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using Cezzi.Applications.Extensions;
using Cezzi.Applications.Text;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class CocktailIngredient : ValueObject
{
    [JsonInclude]
    public string IngredientId { get; private set; }

    [JsonInclude]
    public string VariationId { get; private set; }

    [JsonInclude]
    public float Units { get; private set; }

    [JsonInclude]
    public UofM UoM { get; private set; }

    [JsonInclude]
    public PreparationType Preparation { get; private set; }

    [JsonInclude]
    public string Suggestions { get; private set; }

    [JsonInclude]
    public IngredientRequirement Requirement { get; private set; }

    [JsonInclude]
    public IReadOnlyList<string> Types { get; private set; } = [];

    [JsonInclude]
    public IReadOnlyList<string> Applications { get; private set; } = [];

    [JsonInclude]
    public string Name { get; private set; }

    [JsonInclude]
    public string BaseName { get; private set; }

    [JsonInclude]
    public string ParentIngredientId { get; private set; }

    [JsonConstructor]
    protected CocktailIngredient() { }

    public CocktailIngredient(
        Ingredient ingredient,
        float units,
        UofM unitOfMeasure,
        PreparationType prep = PreparationType.None,
        string suggestions = "",
        IngredientRequirement requirement = IngredientRequirement.Required) : this(ingredient, null, units, unitOfMeasure, prep, suggestions, requirement) { }

    public CocktailIngredient(
        Ingredient ingredient,
        string variationId,
        float units,
        UofM unitOfMeasure,
        PreparationType prep = PreparationType.None,
        string suggestions = "",
        IngredientRequirement requirement = IngredientRequirement.Required) : this()
    {
        this.IngredientId = ingredient.Id ?? throw new CocktailsApiDomainException($"{nameof(ingredient)} cannot be null");

        if (!string.IsNullOrWhiteSpace(variationId))
        {
            var variation = ingredient.Variations.FirstOrDefault(x => x.Id == variationId);
            this.VariationId = variation?.Id ?? throw new CocktailsApiDomainException($"{nameof(variationId)} must be a valid variation id for the base ingredient {ingredient?.Id}");
        }
        else
        {
            this.VariationId = null;
        }

        this.UoM = unitOfMeasure != UofM.None
            ? unitOfMeasure
            : throw new CocktailsApiDomainException($"{nameof(unitOfMeasure)} must be specified");

        this.Requirement = requirement != IngredientRequirement.None
            ? requirement
            : throw new CocktailsApiDomainException($"{nameof(requirement)} must be specified");

        if (units > 0)
        {
            this.Units = units;
        }
        else if (units == 0 && ingredient.Applications.Contains(IngredientApplication.Base.ToString(), StringComparer.OrdinalIgnoreCase) && unitOfMeasure == UofM.Discretion)
        {
            this.Units = units;
        }
        else if (units == 0 && IListExtensions.IsIn(ingredient.Applications, [IngredientApplication.Garnishment.ToString(), IngredientApplication.Muddle.ToString()]))
        {
            this.Units = units;
        }
        else if (units == 0 && ObjectExtensions.IsIn(unitOfMeasure, [UofM.Topoff, UofM.Splash, UofM.ToTaste]))
        {
            this.Units = units;
        }
        else
        {
            throw new CocktailsApiDomainException($"{nameof(units)} must be greater than zero when ingredient is a non-base and not a discretionary unit of measure or ingredient type");
        }

        this.Units = units;
        this.Suggestions = suggestions;
        this.Preparation = prep;
        this.SetBaseIngredient(ingredient);
    }

    public CocktailIngredient SetBaseIngredient(Ingredient ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient, nameof(ingredient));

        this.BaseName = ingredient.Name ?? throw new CocktailsApiDomainException($"{nameof(ingredient)} must have a name");
        this.ParentIngredientId = ingredient.ParentId;

        this.Name = this.VariationId != null
            ? ingredient.Variations.First(x => x.Id == this.VariationId).Name ?? ingredient.Name
            : ingredient.Name;

        this.Applications = this.VariationId != null
            ? ingredient.Variations.First(x => x.Id == this.VariationId).Applications ?? ingredient.Applications
            : ingredient.Applications;

        this.Types = ingredient.Types ?? [];

        return this;
    }

    public string GetMeasurementDisplay(bool condensed = true)
    {
        if (this.UoM == UofM.Discretion)
        {
            return string.Empty;
        }

        if (this.UoM == UofM.Ounces)
        {
            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8 {(condensed ? "oz" : "ounce")}",
                    0.20F => $"1/5 {(condensed ? "oz" : "ounce")}",
                    0.25F => $"1/4 {(condensed ? "oz" : "ounce")}",
                    0.33F => $"1/3 {(condensed ? "oz" : "ounce")}",
                    0.5F => $"1/2 {(condensed ? "oz" : "ounce")}",
                    0.75F => $"3/4 {(condensed ? "oz" : "ounce")}",
                    1.0F => $"1 {(condensed ? "oz" : "ounce")}",
                    _ => $"{this.Units} {(condensed ? "oz" : "ounce")}"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt} {(condensed ? "oz" : "ounces")}"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8 {(condensed ? "oz" : "ounces")}",
                        0.20F => $"{whole} 1/5 {(condensed ? "oz" : "ounces")}",
                        0.25F => $"{whole} 1/4 {(condensed ? "oz" : "ounces")}",
                        0.33F => $"{whole} 1/3 {(condensed ? "oz" : "ounces")}",
                        0.5F => $"{whole} 1/2 {(condensed ? "oz" : "ounces")}",
                        0.75F => $"{whole} 3/4 {(condensed ? "oz" : "ounces")}",
                        1.0F => $"{whole} 1 {(condensed ? "oz" : "ounces")}",
                        _ => $"{this.Units} {(condensed ? "oz" : "ounces")}"
                    };
            }
        }

        if (this.UoM == UofM.Cups)
        {
            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8 cup",
                    0.20F => $"1/5 cup",
                    0.25F => $"1/4 cup",
                    0.33F => $"1/3 cup",
                    0.5F => $"1/2 cup",
                    0.75F => $"3/4 cup",
                    1.0F => $"1 cup",
                    _ => $"{this.Units} cup"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt} cups"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8 cups",
                        0.20F => $"{whole} 1/5 cups",
                        0.25F => $"{whole} 1/4 cups",
                        0.33F => $"{whole} 1/3 cups",
                        0.5F => $"{whole} 1/2 cups",
                        0.75F => $"{whole} 3/4 cups",
                        1.0F => $"{whole} 1 cups",
                        _ => $"{this.Units} cups"
                    };
            }
        }

        if (this.UoM == UofM.Dashes)
        {
            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8 dash",
                    0.20F => $"1/5 dash",
                    0.25F => $"1/4 dash",
                    0.33F => $"1/3 dash",
                    0.5F => $"1/2 dash",
                    0.75F => $"3/4 dash",
                    1.0F => $"1 dash",
                    _ => $"{this.Units} dash"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt} dashes"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8 dashes",
                        0.20F => $"{whole} 1/5 dashes",
                        0.25F => $"{whole} 1/4 dashes",
                        0.33F => $"{whole} 1/3 dashes",
                        0.5F => $"{whole} 1/2 dashes",
                        0.75F => $"{whole} 3/4 dashes",
                        1.0F => $"{whole} 1 dashes",
                        _ => $"{this.Units} dashes"
                    };
            }
        }

        if (this.UoM == UofM.Tablespoon)
        {
            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8 {(condensed ? "tbsp" : "tablespoon")}",
                    0.20F => $"1/5 {(condensed ? "tbsp" : "tablespoon")}",
                    0.25F => $"1/4 {(condensed ? "tbsp" : "tablespoon")}",
                    0.33F => $"1/3 {(condensed ? "tbsp" : "tablespoon")}",
                    0.5F => $"1/2 {(condensed ? "tbsp" : "tablespoon")}",
                    0.75F => $"3/4 {(condensed ? "tbsp" : "tablespoon")}",
                    1.0F => $"1 {(condensed ? "tbsp" : "tablespoon")}",
                    _ => $"{this.Units} {(condensed ? "tbsp" : "tablespoon")}"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt} {(condensed ? "tbsp" : "tablespoons")}"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8 {(condensed ? "tbsp" : "tablespoons")}",
                        0.20F => $"{whole} 1/5 {(condensed ? "tbsp" : "tablespoons")}",
                        0.25F => $"{whole} 1/4 {(condensed ? "tbsp" : "tablespoons")}",
                        0.33F => $"{whole} 1/3 {(condensed ? "tbsp" : "tablespoons")}",
                        0.5F => $"{whole} 1/2 {(condensed ? "tbsp" : "tablespoons")}",
                        0.75F => $"{whole} 3/4 {(condensed ? "tbsp" : "tablespoons")}",
                        1.0F => $"{whole} 1 {(condensed ? "tbsp" : "tablespoons")}",
                        _ => $"{this.Units} {(condensed ? "tbsp" : "tablespoons")}"
                    };
            }
        }

        if (this.UoM == UofM.Teaspoon)
        {
            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8 {(condensed ? "tsp" : "teaspoon")}",
                    0.20F => $"1/5 {(condensed ? "tsp" : "teaspoon")}",
                    0.25F => $"1/4 {(condensed ? "tsp" : "teaspoon")}",
                    0.33F => $"1/3 {(condensed ? "tsp" : "teaspoon")}",
                    0.5F => $"1/2 {(condensed ? "tsp" : "teaspoon")}",
                    0.75F => $"3/4 {(condensed ? "tsp" : "teaspoon")}",
                    1.0F => $"1 {(condensed ? "tsp" : "teaspoon")}",
                    _ => $"{this.Units} {(condensed ? "tsp" : "teaspoon")}"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt} {(condensed ? "tsp" : "teaspoons")}"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8 {(condensed ? "tsp" : "teaspoons")}",
                        0.20F => $"{whole} 1/5 {(condensed ? "tsp" : "teaspoons")}",
                        0.25F => $"{whole} 1/4 {(condensed ? "tsp" : "teaspoons")}",
                        0.33F => $"{whole} 1/3 {(condensed ? "tsp" : "teaspoons")}",
                        0.5F => $"{whole} 1/2 {(condensed ? "tsp" : "teaspoons")}",
                        0.75F => $"{whole} 3/4 {(condensed ? "tsp" : "teaspoons")}",
                        1.0F => $"{whole} 1 {(condensed ? "tsp" : "teaspoons")}",
                        _ => $"{this.Units} {(condensed ? "tsp" : "teaspoons")}"
                    };
            }
        }

        if (this.UoM == UofM.Barspoon)
        {
            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8 barspoon",
                    0.20F => $"1/5 barspoon",
                    0.25F => $"1/4 barspoon",
                    0.33F => $"1/3 barspoon",
                    0.5F => $"1/2 barspoon",
                    0.75F => $"3/4 barspoon",
                    1.0F => $"1 barspoon",
                    _ => $"{this.Units} barspoon"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt} barspoons"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8 barspoons",
                        0.20F => $"{whole} 1/5 barspoons",
                        0.25F => $"{whole} 1/4 barspoons",
                        0.33F => $"{whole} 1/3 barspoons",
                        0.5F => $"{whole} 1/2 barspoons",
                        0.75F => $"{whole} 3/4 barspoons",
                        1.0F => $"{whole} 1 barspoons",
                        _ => $"{this.Units} barspoons"
                    };
            }
        }

        if (this.UoM == UofM.Item)
        {
            if (this.Applications.Contains(IngredientApplication.Base.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (this.Applications.Contains(IngredientApplication.Garnishment.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (this.Applications.Contains(IngredientApplication.Additional.ToString(), StringComparer.OrdinalIgnoreCase) && (this.UoM != UofM.Item || this.Units <= 0))
            {
                return string.Empty;
            }

            if (this.Units <= 1F)
            {
                return this.Units switch
                {
                    0.125F => $"1/8",
                    0.20F => $"1/5",
                    0.25F => $"1/4",
                    0.33F => $"1/3",
                    0.5F => $"1/2",
                    0.75F => $"3/4",
                    1.0F => $"1",
                    _ => $"{this.Units}"
                };
            }
            else if (this.Units > 1F)
            {
                var whole = Math.Truncate(this.Units);
                var wholeInt = Convert.ToInt32(whole);

                return whole == this.Units
                    ? $"{wholeInt}"
                    : (this.Units - whole) switch
                    {
                        0.125F => $"{whole} 1/8",
                        0.20F => $"{whole} 1/5",
                        0.25F => $"{whole} 1/4",
                        0.33F => $"{whole} 1/3",
                        0.5F => $"{whole} 1/2",
                        0.75F => $"{whole} 3/4",
                        1.0F => $"{whole} 1",
                        _ => $"{this.Units}"
                    };
            }
        }

        return this.UoM == UofM.ToTaste ? string.Empty : this.UoM == UofM.Topoff ? string.Empty : string.Empty;
    }

    public string GetIngredientSuffix()
    {
        return this.UoM == UofM.ToTaste
            ? "to taste"
            : this.Applications.Contains(IngredientApplication.Garnishment.ToString(), StringComparer.OrdinalIgnoreCase)
            ? "for garnishment"
            : this.UoM == UofM.Topoff
            ? "(top off)"
            : this.Applications.Contains(IngredientApplication.Muddle.ToString(), StringComparer.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(this.Suggestions)
            ? $"({this.Suggestions.ToLower()})"
            : string.Empty;
    }

    public string GetDisplayValue() => $"{this.GetMeasurementDisplay()} {this.Name} {this.GetIngredientSuffix()}".Trim();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.IngredientId;
        yield return this.VariationId;
        yield return this.Units;
        yield return this.UoM;
        yield return this.Preparation;
        yield return this.Suggestions;
        yield return this.Requirement;
    }

    public string GenerateHash()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(
            string.Join(',', this.Types) +
            string.Join(',', this.Applications) +
            this.IngredientId +
            this.ParentIngredientId ?? "Na" +
            this.Name +
            this.BaseName +
            this.UoM.ToString() +
            this.Units.ToString() +
            this.Suggestions ?? "Na" +
            this.Requirement.ToString() +
            this.Preparation ?? "Na" +
            this.VariationId ?? "Na");

        return Base64.Encode(bytes).GetHashCode().ToString();
    }
}
