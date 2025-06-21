namespace Cocktails.Api.Domain;

public class AccountMonikers
{
#pragma warning disable CA1822 // Mark members as static
    public string AccountId => "@cz_account_id";

    public string SubjectId => "@cz_account_sub";

    public string Name => "@cz_account_name";
#pragma warning restore CA1822 // Mark members as static
}
