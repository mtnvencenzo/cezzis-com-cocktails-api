namespace Cocktails.Api.Application.Behaviors.OAuthScopeAuthorization;

using System.Reflection;

public class AuthScopes
{
    public const string AdminCezziCocktails = "admin:cezzi-cocktails";

    public static string[] All()
    {
        return [.. typeof(AuthScopes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly) // IsLiteral for compile-time constants, !IsInitOnly to exclude static readonly
            .Select(f => f.GetValue(null).ToString())];
    }
}
