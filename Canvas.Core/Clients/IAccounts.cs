using Canvas.Core.Entities;
using Canvas.Core.Settings;

namespace Canvas.Core.Clients;

/// <summary>
/// Provides access to the account-related functionality.
/// </summary>
public interface IAccounts
{
    /// <summary>
    /// Gets the terms interface.
    /// </summary>
    ITerms Terms { get; }

    /// <summary>
    /// Lists all the accounts for the current user.
    /// </summary>
    /// <param name="opts">The options for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Account}"/> containing the accounts for the current user.</returns>
    IAsyncEnumerable<Account> ListForCurrentUser(List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of an account.
    /// </summary>
    /// <param name="id">The identifier of the account.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Account"/> instance if found; <c>null</c> otherwise.</returns>
    Task<Account?> Retrieve(ulong id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of an account.
    /// </summary>
    /// <param name="account">The <see cref="account"/> instance to retrieve the details for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Account"/> instance if found; <c>null</c> otherwise.</returns>
    Task<Account?> Retrieve(Account account, CancellationToken cancellationToken = default);
}