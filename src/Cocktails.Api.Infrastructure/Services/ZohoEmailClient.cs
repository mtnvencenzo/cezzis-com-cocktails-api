namespace Cocktails.Api.Infrastructure.Services;

using Cezzi.Applications.Extensions;
using Cezzi.Smtp;
using Cocktails.Api.Domain;
using Cocktails.Api.Domain.Config;
using Cocktails.Common.Emails;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public class ZohoEmailClient(
    IOptions<ZohoEmailConfig> zohoEmailConfig,
    ISmtpClientFactory smtpClientFactory,
    ILogger<ZohoEmailClient> logger) : IZohoEmailClient
{
    private readonly ZohoEmailConfig zohoEmailConfig = zohoEmailConfig?.Value ?? throw new ArgumentNullException(nameof(zohoEmailConfig));
    private readonly ISmtpClientFactory smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));
    private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task SendEmail(EmailMessage message, CancellationToken cancellationToken)
    {
        using var emailScope = this.logger.BeginScope(new Dictionary<string, object>
        {
            { Monikers.Email.SmtpHost, this.zohoEmailConfig.SmtpHost },
            { Monikers.Email.SmtpPort, this.zohoEmailConfig.SmtpPort },
            { Monikers.Email.EmailSubject, message.Subject },
            { Monikers.Email.EmailFrom, message.From?.FullAddress() },
            { Monikers.Email.EmailTo, message.To?.JoinString() },
            { Monikers.Email.EmailCc, message.Cc?.JoinString() },
            { Monikers.Email.EmailBcc, message.Bcc?.JoinString() },
            { Monikers.Email.EmailReplyTo, message.ReplyTo?.FullAddress() },
        });

        if (string.IsNullOrWhiteSpace(message.Subject) && string.IsNullOrWhiteSpace(message.Body))
        {
            this.logger.LogWarning("Email subject and body are empty, skipping sending email");
            return;
        }

        if (string.IsNullOrWhiteSpace(message.From?.Address))
        {
            this.logger.LogWarning("Email from address is empty, skipping sending email");
            return;
        }

        if (string.IsNullOrWhiteSpace(message.To?.FirstOrDefault()?.Address))
        {
            this.logger.LogWarning("Email to address is empty, skipping sending email");
            return;
        }

        if (!string.Equals(this.zohoEmailConfig.DefaultSender.EmailAddress, message.From.Address, StringComparison.OrdinalIgnoreCase))
        {
            this.logger.LogWarning("Mismatched sender email, skipping sending email");
            return;
        }

        using var client = this.smtpClientFactory.CreateClient(
            host: this.zohoEmailConfig.SmtpHost,
            port: this.zohoEmailConfig.SmtpPort);

        client.EnableSsl = true;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(this.zohoEmailConfig.DefaultSender.EmailAddress, this.zohoEmailConfig.DefaultSender.AppPassword);

        var mailMessage = new MailMessage
        {
            Subject = message.Subject.NullIfNullOrWhiteSpace(),
            Body = message.Body.NullIfNullOrWhiteSpace(),
            IsBodyHtml = true,
            From = new MailAddress(this.zohoEmailConfig.DefaultSender.EmailAddress, message.From.DisplayName),
            Sender = new MailAddress(message.From.Address, message.From.DisplayName),
            Priority = Enum.Parse<MailPriority>(message.Priority.ToString())
        };
        message.To?
            .Where(x => !string.IsNullOrWhiteSpace(x?.Address))
            .ToList()
            .ForEach(x => mailMessage.To.Add(new MailAddress(x.Address, x.DisplayName)));
        message.Cc?
            .Where(x => !string.IsNullOrWhiteSpace(x?.Address))
            .ToList()
            .ForEach(x => mailMessage.CC.Add(new MailAddress(x.Address, x.DisplayName)));
        message.Bcc?
            .Where(x => !string.IsNullOrWhiteSpace(x?.Address))
            .ToList()
            .ForEach(x => mailMessage.Bcc.Add(new MailAddress(x.Address, x.DisplayName)));

        try
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await client.SendMailAsync(
                    message: mailMessage,
                    cancellationToken: cancellationToken);

                this.logger.LogInformation("Email sent");
            }
            else
            {
                this.logger.LogWarning("Cancellation requested, skipping sending email");
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Email send failed");
            throw;
        }
    }
}
