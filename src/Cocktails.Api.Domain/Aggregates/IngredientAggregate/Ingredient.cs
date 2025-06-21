namespace Cocktails.Api.Domain.Aggregates.IngredientAggregate;

using Cezzi.Applications.Extensions;
using Cezzi.Applications.Text;
using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Ingredient : Entity, IAggregateRoot
{
#pragma warning disable IDE0032 // Use auto property
    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<string> types;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<string> applications;

    [JsonInclude, JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    private readonly List<IngredientVariation> variations;
#pragma warning restore IDE0032 // Use auto property

    [JsonInclude]
    public string ParentId { get; private set; }

    [JsonInclude]
    public string Name { get; private set; }

    [JsonInclude]
    public string ETag { get; private set; }

    [JsonInclude]
    public string ShelfDisplay { get; private set; }

    [JsonInclude]
    public string Discriminator { get; private set; }

    [JsonInclude]
    public string Hash { get; private set; }

    [JsonIgnore]
    public List<string> Types => this.types;

    [JsonIgnore]
    public List<string> Applications => this.applications;

    [JsonIgnore]
    public IReadOnlyCollection<IngredientVariation> Variations => this.variations;

    [JsonConstructor]
    protected Ingredient()
    {
        this.types = [];
        this.applications = [];
        this.variations = [];
    }

    public Ingredient(
        string id,
        string name,
        string shelfDisplay,
        List<string> types,
        List<string> applications,
        DateTimeOffset createdOn,
        DateTimeOffset updatedOn) : this(id, null, name, shelfDisplay, types, applications, [], createdOn, updatedOn) { }

    public Ingredient(
        string id,
        string name,
        string shelfDisplay,
        List<string> types,
        List<string> applications,
        List<IngredientVariation> variations,
        DateTimeOffset createdOn,
        DateTimeOffset updatedOn) : this(id, null, name, shelfDisplay, types, applications, variations, createdOn, updatedOn) { }

    public Ingredient(
        string id,
        string parentId,
        string name,
        string shelfDisplay,
        List<string> types,
        List<string> applications,
        List<IngredientVariation> variations,
        DateTimeOffset createdOn,
        DateTimeOffset updatedOn) : this()
    {
        this.Id = !string.IsNullOrWhiteSpace(id)
            ? id
            : throw new CocktailsApiDomainException($"{nameof(id)} cannot be null or empty");

        this.SetName(name)
            .SetShelfDisplay(shelfDisplay)
            .SetTypes(types ?? [])
            .SetApplications(applications ?? [])
            .SetParentId(parentId)
            .SetVariations(variations ?? []);

        if (createdOn > updatedOn)
        {
            throw new CocktailsApiDomainException($"Created on {nameof(createdOn)} cannot be greater than updated on {nameof(updatedOn)}");
        }

        this.UpdatedOn = updatedOn;
        this.CreatedOn = createdOn;
    }

    public Ingredient MergeUpdate(Ingredient from)
    {
        this.SetName(from.Name)
            .SetShelfDisplay(from.ShelfDisplay)
            .SetTypes(from.types ?? [])
            .SetApplications(from.applications ?? [])
            .SetParentId(from.ParentId)
            .SetVariations(from.variations ?? [])
            .SetHash(from.Hash);

        this.UpdatedOn = DateTimeOffset.Now;
        return this;
    }

    private Ingredient SetParentId(string parentId)
    {
        this.ParentId = !string.IsNullOrWhiteSpace(parentId)
            ? parentId
            : null;

        return this;
    }

    private Ingredient SetName(string name)
    {
        this.Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new CocktailsApiDomainException($"{nameof(name)} cannot be null or empty");

        return this;
    }

    private Ingredient SetShelfDisplay(string shelfDisplay)
    {
        this.ShelfDisplay = !string.IsNullOrWhiteSpace(shelfDisplay)
            ? shelfDisplay
            : throw new CocktailsApiDomainException($"{nameof(shelfDisplay)} cannot be null or empty");

        return this;
    }

    private Ingredient SetTypes(List<string> types)
    {
        if (types != null && types.Count > 0 && !types.Contains(IngredientType.None.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            this.types.Clear();
            this.types.AddRange(types.Distinct());
        }
        else
        {
            throw new CocktailsApiDomainException($"{nameof(types)} must contain at least one specified type");
        }

        return this;
    }

    private Ingredient SetApplications(List<string> applications)
    {
        if (applications != null && applications.Count > 0 && !applications.Contains(IngredientApplication.None.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            this.applications.Clear();
            this.applications.AddRange(applications.Distinct());
        }
        else
        {
            throw new CocktailsApiDomainException($"{nameof(applications)} must contain at least one specified application");
        }

        return this;
    }

    private Ingredient SetVariations(List<IngredientVariation> variations)
    {
        this.variations.Clear();

        if (variations != null && variations.Count > 0)
        {
            this.variations.AddRange(variations.DistinctBy(x => x.Id));
        }

        return this;
    }

    private Ingredient SetHash(string hash)
    {
        this.Hash = !string.IsNullOrWhiteSpace(hash)
            ? hash
            : throw new CocktailsApiDomainException($"{nameof(hash)} cannot be null or empty");

        return this;
    }

    public string RegenerateHash()
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(
            string.Join(',', this.types) +
            string.Join(',', this.applications) +
            this.Id +
            this.Name +
            this.ParentId ?? string.Empty +
            this.ShelfDisplay +
            string.Join(',', this.variations.Select(x => x.Id + x.Name + string.Join(',', x.Applications ?? []))));

        this.Hash = Base64.Encode(bytes).GetHashCode().ToString();
        return this.Hash;
    }
}
