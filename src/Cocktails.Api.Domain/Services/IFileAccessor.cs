namespace Cocktails.Api.Domain.Services;

public interface IFileAccessor
{
    Stream GetStream();

    long GetLength();

    string GetContentType();

    string GetFileName();
}
