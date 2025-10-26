namespace Cocktails.Api.Domain.Config;

public class ZohoEmailConfig
{
    public const string SectionName = "ZohoEmail";

    public string SmtpHost { get; set; }

    public int SmtpPort { get; set; }

    public ZohoEmailSenderConfig DefaultSender { get; set; }
}
