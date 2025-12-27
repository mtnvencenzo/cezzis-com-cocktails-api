namespace Cocktails.Api.Domain.Aggregates.IngredientAggregate;

using Cezzi.Applications.Extensions;
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
            .SetVariations(from.variations ?? []);

        this.UpdatedOn = DateTimeOffset.Now;
        return this;
    }

    public bool IsSameAs(Ingredient other)
    {
        if (other == null)
        {
            return false;
        }

        if (this.Id != other.Id ||
            this.Name != other.Name ||
            this.ParentId != other.ParentId ||
            this.ShelfDisplay != other.ShelfDisplay ||
            this.types.Count != other.types.Count ||
            this.applications.Count != other.applications.Count ||
            this.variations.Count != other.variations.Count)
        {
            return false;
        }

        for (var i = 0; i < this.types.Count; i++)
        {
            if (this.types[i] != other.types[i])
            {
                return false;
            }
        }

        for (var i = 0; i < this.applications.Count; i++)
        {
            if (this.applications[i] != other.applications[i])
            {
                return false;
            }
        }

        for (var i = 0; i < this.variations.Count; i++)
        {
            if (!this.variations[i].IsSameAs(other.variations[i]))
            {
                return false;
            }
        }

        return true;
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
}
