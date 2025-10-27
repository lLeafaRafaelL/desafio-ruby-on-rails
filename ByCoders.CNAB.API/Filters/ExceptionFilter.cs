using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Infrastructure.Correlation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ByCoders.CNAB.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger _logger;
    private readonly ICorrelationService _correlation;

    public ExceptionFilter(ILogger logger, ICorrelationService correlation)
    {
        _logger = logger;
        _correlation = correlation;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Erro Interno.", _correlation.GetCorrelationId().ToString());

        context.Result = new BadRequestObjectResult(
        new List<ResultFailureDetail> { new ResultFailureDetail("Erro interno.", "ERR999") })
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true;
    }
}