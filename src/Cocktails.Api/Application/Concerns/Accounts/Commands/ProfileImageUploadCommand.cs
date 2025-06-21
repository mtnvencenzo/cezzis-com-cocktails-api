namespace Cocktails.Api.Application.Concerns.Accounts.Commands;

using Cezzi.Applications;
using FluentValidation;
using global::Cocktails.Api.Application.Concerns.Accounts.Models;
using global::Cocktails.Api.Application.Utilities;
using global::Cocktails.Api.Domain.Aggregates.AccountAggregate;
using global::Cocktails.Api.Domain.Config;
using global::Cocktails.Api.Domain.Services;
using MediatR;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

[type: Description("The profile image upload command")]
public record ProfileImageUploadCommand
(
    [property: Required()]
    [property: Description("The profile image uploaded file")]
    IFormFile FormFile,

    [property: Description("The claims identity for the user")]
    ClaimsIdentity Identity

) : IRequest<UploadProfileImageRs>;

public class ProfileImageUploadCommandHandler(
    IAccountRepository accountRepository,
    IStorageBus storageBus,
    IOptions<BlobStorageConfig> blobStorageConfig,
    ILogger<ProfileImageUploadCommandHandler> logger) : IRequestHandler<ProfileImageUploadCommand, UploadProfileImageRs>
{
    public async Task<UploadProfileImageRs> Handle(ProfileImageUploadCommand command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetOrCreateLocalAccountFromIdentity(
            claimsIdentity: command.Identity,
            cancellationToken: cancellationToken);

        // Will be used later to destroy the previous blob
        var previousAvatar = account.AvatarUri;

        var bytes = await command.FormFile.GetFileBytes(cancellationToken);

        var imageUri = await storageBus.UpsertBlobAsync(
            data: bytes,
            blobName: $"{account.Id}/{Guid.NewGuid()}{Path.GetExtension(command.FormFile.FileName)?.ToLower()}",
            contentType: command.FormFile.ContentType,
            contentDisposition: null,
            bindingName: blobStorageConfig.Value.AccountAvatars.DaprBuildingBlock,
            cancellationToken: cancellationToken,
            metadata: new Dictionary<string, string>
            {
                { "accountid", account.Id },
                { "subject", account.SubjectId }
            });

        if (!string.IsNullOrWhiteSpace(imageUri))
        {
            var cdnSwapped = new Uri(imageUri).ReplaceHostName(blobStorageConfig.Value.CdnHostName);
            imageUri = cdnSwapped.ToString();
            account.SetAvatarUri(cdnSwapped.ToString());
            await accountRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(previousAvatar))
        {
            var previousAvatarBlobName = new Uri(previousAvatar).AbsolutePath.TrimStart('/');
            previousAvatarBlobName = new Uri(previousAvatar).AbsolutePath[previousAvatarBlobName.IndexOf(account.Id)..].TrimStart('/');

            try
            {
                await storageBus.DeleteBlobAsync(
                    blobName: previousAvatarBlobName,
                    bindingName: blobStorageConfig.Value.AccountAvatars.DaprBuildingBlock,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to delete previous account avatar {BlobName}", previousAvatarBlobName);
            }
        }

        return new UploadProfileImageRs(imageUri);
    }
}

public class ProfileImageUploadCommandValidator : AbstractValidator<ProfileImageUploadCommand>
{
    /// <summary></summary>
    public ProfileImageUploadCommandValidator()
    {
        this.RuleFor(command => command.FormFile).NotNull().WithMessage("Uploaded profile image file cannot be null");
        this.RuleFor(command => command.FormFile.Length).GreaterThan(0).WithMessage("Uploaded profile image file cannot be empty");
    }
}
