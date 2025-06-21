namespace Cocktails.Api.Apis.LocalImages;

using Cocktails.Api.Application.Concerns.LocalImages.Queries;

/// <summary>
/// 
/// </summary>
/// <param name="queries"></param>
public class LocalImagesServices(ILocalImagesQueries queries)
{
    /// <summary></summary>
    public ILocalImagesQueries Queries { get; } = queries ?? throw new ArgumentNullException(nameof(queries));
}