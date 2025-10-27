namespace ByCoders.CNAB.Core;

public enum RequestHandlerStatus : byte
{
    OK,
    Created,
    Accepted,
    NoContent,
    Unprocessable,
    NotFound
}