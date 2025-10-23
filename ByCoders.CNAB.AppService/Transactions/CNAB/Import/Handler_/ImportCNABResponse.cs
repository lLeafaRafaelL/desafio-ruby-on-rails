using ByCoders.CNAB.Core;
using System.Collections.Generic;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

public record ImportCNABResponse : ResponseDto
{
    public bool Success { get; set; }
    public int TransactionsImported { get; set; }
    public List<string> Errors { get; set; } = new();
}