namespace Cocktails.Api.Domain;

public class EmailMonikers
{
#pragma warning disable CA1822 // Mark members as static
    public string SmtpHost => "@cz_email_smtp_host";

    public string SmtpPort => "@cz_email_smtp_port";

    public string EmailTo => "@cz_email_to";

    public string EmailCc => "@cz_email_cc";

    public string EmailBcc => "@cz_email_bcc";

    public string EmailFrom => "@cz_email_from";

    public string EmailReplyTo => "@cz_email_replyto";

    public string EmailSubject => "@cz_email_subject";
#pragma warning restore CA1822 // Mark members as static
}
