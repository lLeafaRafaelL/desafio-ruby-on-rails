using ByCoders.CNAB.Core.Extensions;
using ByCoders.CNAB.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ByCoders.CNAB.Core.Http;

public abstract class BaseController : ControllerBase
{
    private static readonly Dictionary<RequestHandlerStatus, HttpStatusCode> StatusMap = new()
    {
        { RequestHandlerStatus.OK, HttpStatusCode.OK },
        { RequestHandlerStatus.NoContent, HttpStatusCode.NoContent },
        { RequestHandlerStatus.Created, HttpStatusCode.Created },
        { RequestHandlerStatus.Accepted, HttpStatusCode.Accepted },

        { RequestHandlerStatus.Unprocessable, HttpStatusCode.UnprocessableEntity },
        { RequestHandlerStatus.NotFound, HttpStatusCode.NotFound }
    };

    protected readonly ILogger _logger;

    protected BaseController(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected IActionResult ResponseToActionResult<TResultValue>(RequestHandlerResult<TResultValue> response, Func<RequestHandlerResult<TResultValue>, object> mapSuccessRequestBody, Func<RequestHandlerResult<TResultValue>, object> mapFailureRequestBody)
            where TResultValue : Dto
    {
        var httpStatus = StatusMap[response.Status];
        var mapReqBody = response.Succeeded
            ? mapSuccessRequestBody
            : mapFailureRequestBody;

        var reqBody = mapReqBody?.Invoke(response);
        var hasReqBody = reqBody != null;
        var result = hasReqBody
            ? new ObjectResult(reqBody) { StatusCode = (int)httpStatus }
            : (IActionResult)new StatusCodeResult((int)httpStatus);

        if (!response.Succeeded && hasReqBody)
        {
            _logger.LogInformation(reqBody.ToJson());
        }

        return result;
    }

    protected IActionResult UseCaseResultToActionResult<TResultValue>(RequestHandlerResult<TResultValue?> response, Func<RequestHandlerResult<TResultValue?>, object> mapSuccessRequestBody)
        where TResultValue : Dto =>
            ResponseToActionResult(response, mapSuccessRequestBody, x => x.FailureDetails);
}