namespace ByCoders.CNAB.Core.Results;

/// <summary>
/// Supports situations where it is undesirable to throw exceptions, such as business rule validation on web servers (overhead).
/// </summary>
public readonly struct ResultFailureDetail
{
    public string Tag { get; }

    public string? Description { get; }

    public ResultFailureDetail(string? description, string? tag = null)
    {
        Description = description;
        Tag = string.IsNullOrEmpty(tag) ? "__general__" : tag;
    }

    public static ResultFailureDetail[] EmptyFailureDetails() => Array.Empty<ResultFailureDetail>();

    public static implicit operator ResultFailureDetail(string description) => new(description);
}