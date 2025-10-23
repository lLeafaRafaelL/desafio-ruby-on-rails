using ByCoders.CNAB.Core;
using Microsoft.AspNetCore.Http;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

public record ImportCNABRequest : RequestDto
{
    public IFormFile CNABFile { get; set; }
}
