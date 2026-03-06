using System.Security.Claims;

namespace Ploch.Common.AppServices.Security;

/// <summary>
///     Provides information about the current user.
/// </summary>
public interface IUserInfoProvider
{
    /// <summary>
    ///     Returns the current user.
    /// </summary>
    /// <returns>Current user.</returns>
    ClaimsPrincipal? GetCurrentUserInfo();
}
