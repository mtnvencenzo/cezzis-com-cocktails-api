namespace Cocktails.Api.Infrastructure;

using System.Text;

/// <summary>
/// 
/// </summary>
public static class ExtensionMethods
{
    /// <summary>Froms the bytes.</summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns></returns>
    public static string FromBytes(this byte[] bytes) => Encoding.UTF8.GetString(bytes);
}
