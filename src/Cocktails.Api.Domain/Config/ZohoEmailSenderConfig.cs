namespace Cocktails.Api.Domain.Config;

public class ZohoEmailSenderConfig
{
    public required string EmailAddress { get; set; }

    public required string AppPassword { get; set; }
}
