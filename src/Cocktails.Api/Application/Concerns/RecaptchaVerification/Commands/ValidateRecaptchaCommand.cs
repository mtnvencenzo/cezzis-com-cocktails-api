namespace Cocktails.Api.Application.Concerns.RecaptchaVerification.Commands;

using Cezzi.Security.Recaptcha;
using MediatR;
using Microsoft.Extensions.Options;

public record ValidateRecaptchaCommand(string VerificationToken, string UserIp) : IRequest<RecaptchaVerificationResult>;

public class ValidateRecaptchaCommandHandler(
    IRecaptchaSiteVerifyService recaptchaSiteVerifyService,
    IOptions<RecaptchaConfig> recaptchaConfig) : IRequestHandler<ValidateRecaptchaCommand, RecaptchaVerificationResult>
{
    public async Task<RecaptchaVerificationResult> Handle(ValidateRecaptchaCommand command, CancellationToken cancellationToken)
    {
        var result = await recaptchaSiteVerifyService.Verify(
            verificationCode: command.VerificationToken,
            config: recaptchaConfig.Value,
            userIp: command.UserIp);

        return result;
    }
}

