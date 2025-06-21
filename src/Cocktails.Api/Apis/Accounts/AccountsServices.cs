namespace Cocktails.Api.Apis.Accounts;

using Cocktails.Api.Application.Concerns.Accounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;

public class AccountsServices(IAccountsQueries queries, IHttpContextAccessor httpContextAccessor, IMediator mediator)
{
    /// <summary></summary>
    public IAccountsQueries Queries { get; } = queries ?? throw new ArgumentNullException(nameof(queries));

    /// <summary></summary>
    public IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    /// <summary></summary>
    public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
}