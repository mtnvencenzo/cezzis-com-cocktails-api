namespace Cocktails.Api.Application.Behaviors.Authorization;

using System.Reflection;

public class AuthScopes
{
    public const string ReadOwnedAccount = "read:owned-account";
    public const string WriteOwnedAccount = "write:owned-account";

    public static string[] All()
    {
        return typeof(AuthScopes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly) // IsLiteral for compile-time constants, !IsInitOnly to exclude static readonly
            .Select(f => f.GetValue(null).ToString())
            .ToArray();
    }
}
