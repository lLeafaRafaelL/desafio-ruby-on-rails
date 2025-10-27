using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Core.Results;

namespace ByCoders.CNAB.Application.Files.CNAB.Parsers;

/// <summary>
/// Interface for parsing CNAB format lines
/// </summary>
public interface ICNABLineParser
{
    /// <summary>
    /// Parses a CNAB line and extracts transaction data
    /// </summary>
    /// <param name="line">The CNAB formatted line to parse</param>
    /// <returns>Result containing parsed data or error message</returns>
    Result<CNABFactoryParams> Parse(string line);
}
