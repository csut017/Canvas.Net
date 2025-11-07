using Canvas.Core.Clients;
using Canvas.Core.Entities;
using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;

namespace Canvas.Core.Implementations;

/// <summary>
/// Default implementation of <see cref="ITerms"/>.
/// </summary>
internal class TermsClient
    : ITerms
{
    private readonly IConnection _connection;
    private readonly ILogger? _logger;

    /// <summary>
    /// Initialises a new <see cref="ICurrentUser"/> instance.
    /// </summary>
    /// <param name="connection">The underlying connection.</param>
    /// <param name="logger">An optional logger.</param>
    public TermsClient(IConnection connection, ILogger? logger = null)
    {
        Guard.IsNotNull(connection);
        _connection = connection;
        _logger = logger?.ForContext<CurrentUserClient>();
    }

    /// <summary>
    /// Lists all the terms for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="opts">The options for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Term}"/> containing the terms for the account.</returns>
    public Task<IQueryable<Term>> ListForAccount(ulong accountId, List? opts = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Lists all the terms for an account.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="opts">The options for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Term}"/> containing the terms for the account.</returns>
    public Task<IQueryable<Term>> ListForAccount(Account account, List? opts = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the details of a term.
    /// </summary>
    /// <param name="id">The identifier of the term.</param>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Term"/> instance if found; <c>null</c> otherwise.</returns>
    public Task<Term?> Retrieve(ulong accountId, ulong id, CancellationToken cancellationToken = default)
    {
        _logger?.Debug("Retrieving term with id {termId} in {accountId}", id, accountId);
        return _connection.Retrieve<Term>(
            $"/api/v1/accounts/{accountId}/terms/{id}",
            [],
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the details of a term.
    /// </summary>
    /// <param name="term">The <see cref="Term"/> instance to retrieve the details for.</param>
    /// <param name="account">The account.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Term"/> instance if found; <c>null</c> otherwise.</returns>
    public Task<Term?> Retrieve(Account account, Term term, CancellationToken cancellationToken = default)
    {
        return Retrieve(account.Id, term.Id, cancellationToken);
    }
}