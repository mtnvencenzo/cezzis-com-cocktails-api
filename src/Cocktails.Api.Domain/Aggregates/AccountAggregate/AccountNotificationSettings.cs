namespace Cocktails.Api.Domain.Aggregates.AccountAggregate;

using Cocktails.Api.Domain.Common;
using Cocktails.Api.Domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class AccountNotificationSettings : ValueObject
{
    [JsonInclude]
    public CocktailUpdatedNotification OnNewCocktailAdditions { get; private set; }

    [JsonConstructor]
    protected AccountNotificationSettings() { }

    public AccountNotificationSettings(CocktailUpdatedNotification onNewCocktailAdditions)
    {
        this.SetOnNewCocktailAdditions(onNewCocktailAdditions);
    }

    public AccountNotificationSettings SetOnNewCocktailAdditions(CocktailUpdatedNotification onNewCocktailAdditions)
    {
        if (!Enum.IsDefined(typeof(CocktailUpdatedNotification), onNewCocktailAdditions))
        {
            throw new CocktailsApiDomainException($"{nameof(CocktailUpdatedNotification)} member '{onNewCocktailAdditions}' not found");
        }

        this.OnNewCocktailAdditions = onNewCocktailAdditions;
        return this;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.OnNewCocktailAdditions;
    }
}
