using Canvas.Core.Entities;
using Canvas.Core.Settings;

namespace Canvas.Core.Clients;

/// <summary>
/// Provides access to the term-related functionality.
/// </summary>
public interface ITerms
{
    /// <summary>
    /// Lists all the terms for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="opts">The options for performing the list.</param>
    /// <returns>An <see cref="IQueryable{Term}"/> containing the terms for the account.</returns>
    Task<IQueryable<Term>> ListForAccount(ulong accountId, List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all the terms for an account.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="opts">The options for performing the list.</param>
    /// <returns>An <see cref="IQueryable{Term}"/> containing the terms for the account.</returns>
    Task<IQueryable<Term>> ListForAccount(Account account, List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of a term.
    /// </summary>
    /// <param name="id">The identifier of the term.</param>
    /// <param name="accountId">The identifier of the account.</param>
    /// <returns>A <see cref="Term"/> instance if found; <c>null</c> otherwise.</returns>
    Task<Term?> Retrieve(ulong accountId, ulong id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of a term.
    /// </summary>
    /// <param name="term">The <see cref="Term"/> instance to retrieve the details for.</param>
    /// <param name="account">The account.</param>
    /// <returns>A <see cref="Term"/> instance if found; <c>null</c> otherwise.</returns>
    Task<Term?> Retrieve(Account account, Term term, CancellationToken cancellationToken = default);
}