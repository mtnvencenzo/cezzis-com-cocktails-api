namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AccountNotificationSettings : ValueObject
{
    [JsonInclude]
    public CocktailUpdateNotification NewCocktails { get; private set; }

    [JsonConstructor]
    protected AccountNotificationSettings() { }

    public AccountNotificationSettings(
        CocktailUpdateNotification newCocktails)
    {
        this.NewCocktails = newCocktails;
    }

    public AccountNotificationSettings SetNewCocktailNotification(CocktailUpdateNotification notification)
    {
        var value = (int)notification;

        if (!Enum.GetValues<CocktailUpdateNotification>().Select(x => (int)x).Contains(value))
        {
            throw new CocktailsApiDomainException($"{nameof(CocktailUpdateNotification)} member '{notification}' not found");
        }

        this.NewCocktails = notification;
        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.NewCocktails;
    }
}
