using Canvas.Core.Clients;
using Canvas.Core.Entities;
using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;
using Serilog.Core;

namespace Canvas.Core.Implementations;

/// <summary>
/// Default implementation of <see cref="IAccounts"/>.
/// </summary>
internal class AccountsClient
    : IAccounts
{
    private readonly IConnection _connection;
    private readonly ILogger? _logger;

    private readonly Lazy<ITerms> _termsClient;

    /// <summary>
    /// Initialises a new <see cref="ICurrentUser"/> instance.
    /// </summary>
    /// <param name="connection">The underlying connection.</param>
    /// <param name="logger">An optional logger.</param>
    public AccountsClient(IConnection connection, ILogger? logger = null)
    {
        Guard.IsNotNull(connection);
        _connection = connection;
        _logger = logger?.ForContext<CurrentUserClient>();

        // Initialize the underlying clients
        _termsClient = new Lazy<ITerms>(() => new TermsClient(connection, _logger));
    }

    /// <summary>
    /// Gets the terms interface.
    /// </summary>
    public ITerms Terms => _termsClient.Value;

    /// <summary>
    /// Lists all the accounts for the current user.
    /// </summary>
    /// <param name="opts">The options for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Account}"/> containing the accounts for the current user.</returns>
    public IAsyncEnumerable<Account> ListForCurrentUser(List? opts = null, CancellationToken cancellationToken = default)
    {
        opts ??= new();

        _logger?.Debug("Listing accounts for current user");
        return _connection.List<Account>(
            "/api/v1/accounts",
            opts,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the details of an account.
    /// </summary>
    /// <param name="id">The identifier of the account.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Account"/> instance if found; <c>null</c> otherwise.</returns>
    public Task<Account?> Retrieve(ulong id, CancellationToken cancellationToken = default)
    {
        _logger?.Debug("Retrieving account with id {id}", id);
        return _connection.Retrieve<Account>(
            $"/api/v1/accounts/{id}",
            [],
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the details of an account.
    /// </summary>
    /// <param name="account">The <see cref="account"/> instance to retrieve the details for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Account"/> instance if found; <c>null</c> otherwise.</returns>
    public Task<Account?> Retrieve(Account account, CancellationToken cancellationToken = default)
    {
        return Retrieve(account.Id, cancellationToken);
    }
}