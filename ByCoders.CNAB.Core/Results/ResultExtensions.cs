namespace ByCoders.CNAB.Core.Results;

public static class ResultExtensions
{
    public static string Inline(this IEnumerable<ResultFailureDetail> @this)
    {
        if (@this is null)
        {
            throw new ArgumentNullException(nameof(@this));

        }
        return string.Join(Environment.NewLine, (@this.Select(x => $"{x.Tag} - {x.Description}")));
    }
}