namespace Cocktails.Api.Domain.Config;

public class ZohoEmailConfig
{
    public const string SectionName = "ZohoEmail";

    public required string SmtpHost { get; set; }

    public required int SmtpPort { get; set; }

    public required ZohoEmailSenderConfig DefaultSender { get; set; }
}
