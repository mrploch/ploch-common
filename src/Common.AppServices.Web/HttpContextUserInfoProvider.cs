using System.Security.Claims;
using Ploch.Common.AppServices.Security;

namespace Ploch.Common.AppServices.Web;

/// <summary>
///     Provides the current user information from the <see cref="HttpContext" /> using the
///     <see cref="IHttpContextAccessor" />.
/// </summary>
/// <param name="httpContextAccessor">The http context accessor.</param>
public class HttpContextUserInfoProvider(IHttpContextAccessor httpContextAccessor) : IUserInfoProvider
{
    /// <inheritdoc />
    public ClaimsPrincipal? GetCurrentUserInfo() => httpContextAccessor.HttpContext?.User;
}
