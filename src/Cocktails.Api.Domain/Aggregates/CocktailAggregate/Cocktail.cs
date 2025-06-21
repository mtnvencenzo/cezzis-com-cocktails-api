namespace Cocktails.Api.Domain.Aggregates.CocktailAggregate;

using Cezzi.Applications.Extensions;
using Cezzi.Applications.Text;
using Cocktails.Api.Domain.Aggregates.IngredientAggregate;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Cocktail : Entity, IAggregateRoot
{
    private readonly static char[] vowels = ['a', 'e', 'i', 'o', 'u'];

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<string> searchableTitles;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<string> glassware;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<CocktailIngredient> ingredients;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<CocktailInstructionStep> instructions;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<CocktailImage> images;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<string> eras;

    [JsonConstructor]
    protected Cocktail()
    {
        this.glassware = [];
        this.ingredients = [];
        this.instructions = [];
        this.images = [];
        this.eras = [];
        this.searchableTitles = [];

        this.PrepTimeMinutes = 10;
        this.Serves = 1;
        this.IsIba = false;
    }

    public Cocktail(
        string id,
        string title,
        string descriptiveTitle = "",
        string description = "")
        : this()
    {
        this.Id = id;
        this.Title = title;
        this.DescriptiveTitle = descriptiveTitle;
        this.Description = description;
    }

    [JsonInclude]
    public string Title { get; private set; }

    [JsonInclude]
    public string Content { get; private set; }

    [JsonInclude]
    public string ContentFile { get; private set; }

    [JsonInclude]
    public string DescriptiveTitle { get; private set; }

    [JsonInclude]
    public string Description { get; private set; }

    [JsonInclude]
    public int Serves { get; private set; }

    [JsonInclude]
    public bool IsIba { get; private set; }

    [JsonInclude]
    public DateTimeOffset ModifiedOn { get; private set; }

    [JsonInclude]
    public DateTimeOffset PublishedOn { get; private set; }

    [JsonInclude]
    public int PrepTimeMinutes { get; private set; }

    [JsonInclude]
    public string ETag { get; private set; }

    [JsonInclude]
    public string Discriminator { get; private set; }

    [JsonInclude]
    public CocktailRating Rating { get; private set; }

    [JsonInclude]
    public string Hash { get; private set; }

    [JsonIgnore]
    public IList<string> SearchableTitles => this.searchableTitles;

    [JsonIgnore]
    public IList<string> Eras => this.eras;

    [JsonIgnore]
    public IReadOnlyCollection<CocktailIngredient> Ingredients => this.ingredients;

    [JsonIgnore]
    public IReadOnlyCollection<CocktailInstructionStep> Instructions => this.instructions;

    [JsonIgnore]
    public IList<string> Glassware => this.glassware;

    [JsonIgnore]
    public IReadOnlyCollection<CocktailImage> Images => this.images.AsReadOnly();

    public override DateTimeOffset CreatedOn
    {
        get => this.PublishedOn;
        protected set
        {
            base.CreatedOn = value;
            this.PublishedOn = value;
        }
    }

    public override DateTimeOffset UpdatedOn
    {
        get => this.ModifiedOn;
        protected set
        {
            base.UpdatedOn = value;
            this.ModifiedOn = value;
        }
    }

    public Cocktail MergeUpdate(Cocktail from)
    {
        this.SetContent(from.Content)
            .SetPublishedOn(from.PublishedOn)
            .SetServes(from.Serves)
            .SetGlassware([.. from.glassware.Select(x => Enum.Parse<GlasswareType>(x, true))])
            .SetEras([.. from.Eras])
            .SetSearchableTitles([.. from.searchableTitles])
            .SetPrepTimeMinutes(from.PrepTimeMinutes)
            .SetIsIba(from.IsIba)
            .SetTitle(from.Title)
            .SetContent(from.Content)
            .SetDescription(from.Description)
            .SetDescriptiveTitle(from.DescriptiveTitle)
            .SetContentFile(from.ContentFile)
            .SetImages([.. from.images])
            .SetInstructions([.. from.instructions])
            .SetIngredients([.. from.ingredients])
            .SetHash(from.Hash);

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetContent(string content)
    {
        this.Content = !string.IsNullOrWhiteSpace(content)
            ? content
            : throw new CocktailsApiDomainException($"{nameof(content)} connot be null or empty");

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetTitle(string title)
    {
        this.Title = !string.IsNullOrWhiteSpace(title)
            ? title
            : throw new CocktailsApiDomainException($"{nameof(title)} connot be null or empty");

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetDescription(string description)
    {
        this.Description = !string.IsNullOrWhiteSpace(description)
            ? description
            : throw new CocktailsApiDomainException($"{nameof(description)} connot be null or empty");

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetDescriptiveTitle(string descriptiveTitle)
    {
        this.DescriptiveTitle = !string.IsNullOrWhiteSpace(descriptiveTitle)
            ? descriptiveTitle
            : throw new CocktailsApiDomainException($"{nameof(descriptiveTitle)} connot be null or empty");

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetContentFile(string contentFile)
    {
        this.ContentFile = !string.IsNullOrWhiteSpace(contentFile)
            ? contentFile
            : throw new CocktailsApiDomainException($"{nameof(contentFile)} connot be null or empty");

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetModifiedOn(DateTimeOffset modifiedOn)
    {
        this.ModifiedOn = modifiedOn;
        return this;
    }

    public Cocktail SetPublishedOn(DateTimeOffset publishedOn)
    {
        this.PublishedOn = publishedOn;
        return this;
    }

    public Cocktail SetServes(int serves)
    {
        this.Serves = serves >= 1
            ? serves
            : throw new CocktailsApiDomainException($"{nameof(serves)} must be greater than zero");

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetGlassware(params GlasswareType[] glassware)
    {
        glassware.ForEach(x =>
        {
            if (!this.glassware.Contains(x.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                this.SetModifiedOn(DateTimeOffset.Now);
                this.glassware.Add(x.ToString());
            }
        });

        return this;
    }

    public Cocktail SetEras(params string[] eras)
    {
        eras.ForEach(x =>
        {
            if (!this.eras.Contains(x))
            {
                this.SetModifiedOn(DateTimeOffset.Now);
                this.eras.Add(x);
            }
        });

        return this;
    }

    public Cocktail SetSearchableTitles(params string[] titles)
    {
        titles.ForEach(x =>
        {
            if (!this.searchableTitles.Contains(x))
            {
                this.SetModifiedOn(DateTimeOffset.Now);
                this.searchableTitles.Add(x);
            }
        });

        return this;
    }

    public Cocktail SetImages(params CocktailImage[] images)
    {
        this.images.Clear();

        images.ForEach(x =>
        {
            if (!this.images.Contains(x))
            {
                this.SetModifiedOn(DateTimeOffset.Now);
                this.images.Add(x);
            }
        });

        return this;
    }

    public Cocktail SetInstructions(params CocktailInstructionStep[] instructions)
    {
        this.instructions.Clear();

        instructions.ForEach(x =>
        {
            if (!this.instructions.Contains(x))
            {
                this.SetModifiedOn(DateTimeOffset.Now);
                this.instructions.Add(x);
            }
        });

        return this;
    }

    public Cocktail SetIngredients(params CocktailIngredient[] ingredients)
    {
        this.ingredients.Clear();

        ingredients.ForEach(x =>
        {
            if (!this.ingredients.Contains(x))
            {
                this.SetModifiedOn(DateTimeOffset.Now);
                this.ingredients.Add(x);
            }
        });

        return this;
    }

    public Cocktail SetPrepTimeMinutes(int minutes)
    {
        this.PrepTimeMinutes = minutes;

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    public Cocktail SetIsIba(bool isIba)
    {
        this.IsIba = isIba;

        this.SetModifiedOn(DateTimeOffset.Now);
        return this;
    }

    private Cocktail SetHash(string hash)
    {
        this.Hash = !string.IsNullOrWhiteSpace(hash)
            ? hash
            : throw new CocktailsApiDomainException($"{nameof(hash)} cannot be null or empty");

        return this;
    }

    public string GetIngredientsMarkDownDescription()
    {
        var ingredients = this.Ingredients
            .Select(x =>
            {
                var desc = string.Empty;

                if (x.Applications.Contains(IngredientApplication.Base.ToString(), StringComparer.OrdinalIgnoreCase) && x.Units == 0 && x.UoM == UofM.Discretion)
                {
                    desc = $"- **{x.Name}** {x.GetIngredientSuffix()}";
                }
                else if (x.Applications.Contains(IngredientApplication.Garnishment.ToString(), StringComparer.OrdinalIgnoreCase))
                {
                    var dunno = vowels.Contains(x.Name.ToLowerInvariant().First())
                        ? "an"
                        : "a";

                    desc = $"- **Garnish** with {dunno} {x.Name.ToLowerInvariant()}";
                }
                else if (x.Applications.Contains(IngredientApplication.Muddle.ToString(), StringComparer.OrdinalIgnoreCase))
                {
                    desc = $"- **{x.GetMeasurementDisplay(false)} {x.Name}** {x.GetIngredientSuffix()}";
                }
                else if (x.UoM == UofM.Topoff)
                {
                    desc = $"- **Top off** with {x.Name.ToLowerInvariant()}";
                }
                else if (x.UoM == UofM.Splash)
                {
                    desc = $"- **Splash** of {x.Name.ToLowerInvariant()}";
                }
                else if (x.UoM == UofM.ToTaste)
                {
                    var name = x.Name[0] + x.Name[1..].ToLowerInvariant();
                    desc = $"- **{name}** to taste";
                }
                else
                {
                    desc = x.Applications.Contains(IngredientApplication.Base.ToString(), StringComparer.OrdinalIgnoreCase) || x.Applications.Contains(IngredientApplication.Additional.ToString(), StringComparer.OrdinalIgnoreCase)
                        ? $"- **{x.GetMeasurementDisplay(false)}** {x.Name.ToLowerInvariant()} {x.GetIngredientSuffix()}"
                        : $"- **{x.GetMeasurementDisplay(false)}** {x.Name.ToLowerInvariant()} {x.GetIngredientSuffix()}";
                }

                return desc.Trim();
            })
            .ToList();

        return string.Join(Environment.NewLine, ingredients);
    }

    public string GetInstructionsMarkDownDescription()
    {
        var instructions = this.Instructions
            .Select(x => $"- {x.DisplayValue}".Trim())
            .ToList();

        return string.Join(Environment.NewLine, instructions);
    }

    public string GetIbaDescription()
    {
        return this.IsIba
            ? "<i>Recognized by the International Bartenders Association as a popular cocktail recipe.</i>"
            : string.Empty;
    }

    public Cocktail SetIngredient(
        Ingredient baseIngredient,
        float units,
        UofM unitOfMeasure,
        PreparationType prep = PreparationType.None,
        string suggestions = "",
        IngredientRequirement requirement = IngredientRequirement.Required)
    {
        this.ingredients.Add(new CocktailIngredient(
            ingredient: baseIngredient,
            units: units,
            unitOfMeasure: unitOfMeasure,
            prep: prep,
            suggestions: suggestions,
            requirement: requirement));

        return this;
    }

    public Cocktail IncrementRating(int stars)
    {
        if (stars is < 1 or > 5)
        {
            throw new CocktailsApiDomainException($"{nameof(stars)} must be between 1 and 5");
        }

        this.Rating ??= new CocktailRating(0, 0, 0, 0, 0, 0);

        _ = this.Rating.Increment(stars);

        this.SetModifiedOn(DateTimeOffset.Now);

        return this;
    }

    public string RegenerateHash()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(
            string.Join(',', this.glassware) +
            this.Content +
            this.ContentFile +
            this.Description +
            this.DescriptiveTitle +
            string.Join(',', this.Eras) +
            this.Id +
            this.IsIba.ToString() +
            this.PrepTimeMinutes.ToString() +
            this.PublishedOn.ToString() +
            string.Join(',', this.searchableTitles) +
            this.Serves.ToString() +
            this.Title +
            string.Join(',', this.ingredients.Select(x => x.GenerateHash())) +
            string.Join(',', this.instructions.Select(x => x.Order.ToString() + x.DisplayValue)) +
            string.Join(',', this.images.Select(x => x.Type.ToString() + x.Height.ToString() + x.Width.ToString() + x.Uri.ToString())));

        this.Hash = Base64.Encode(bytes).GetHashCode().ToString();
        return this.Hash;
    }
}