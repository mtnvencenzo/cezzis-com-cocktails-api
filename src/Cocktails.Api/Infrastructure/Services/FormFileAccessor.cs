namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Api.Domain.Services;
using Microsoft.AspNetCore.Http;
using System.IO;

public class FormFileAccessor(IFormFile formFile) : IFileAccessor
{
    public Stream GetStream() => formFile.OpenReadStream();

    public long GetLength() => formFile.Length;

    public string GetContentType() => formFile.ContentType;

    public string GetFileName() => formFile.FileName;
}