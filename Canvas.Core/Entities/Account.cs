namespace Canvas.Core.Entities;

/// <summary>
/// An account in Canvas.
/// </summary>
public record Account
    : EntityWithIdAndName
{
    /// <summary>
    /// The identifier of the parent account.
    /// </summary>
    [JsonPropertyName("parent_account_id")]
    public ulong ParentAccountId { get; set; }

    /// <summary>
    /// The identifier of the root account.
    /// </summary>
    [JsonPropertyName("root_account_id")]
    public ulong RootAccountId { get; set; }
}