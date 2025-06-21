namespace Cocktails.Api.Infrastructure.Services;

using Cocktails.Common.Emails;

public interface IZohoEmailClient
{
    Task SendEmail(EmailMessage message, CancellationToken cancellationToken);
}
