namespace Cocktails.Api.Domain;

using Cezzi.Applications.Logging;
using Cocktails.Common;

public static class Monikers
{
    static Monikers()
    {
        App = new AppMonikers();
        Api = new ApiMonikers();
        ServiceBus = new ServiceBusMonikers();
        Cocktails = new CocktailsMonikers();
        Email = new EmailMonikers();
        Azure = new AzMonikers();
        Account = new AccountMonikers();
    }

    public static AppMonikers App { get; }

    public static ApiMonikers Api { get; }

    public static AzMonikers Azure { get; }

    public static ServiceBusMonikers ServiceBus { get; }

    public static CocktailsMonikers Cocktails { get; }

    public static EmailMonikers Email { get; }

    public static AccountMonikers Account { get; }
}
