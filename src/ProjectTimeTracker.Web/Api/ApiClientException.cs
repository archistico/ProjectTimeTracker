using System.Net;

namespace ProjectTimeTracker.Web.Api;

public class ApiClientException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ApiError { get; }

    public ApiClientException(string message)
        : base(message)
    {
    }

    public ApiClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ApiClientException(HttpStatusCode statusCode, string message, string? apiError = null)
        : base(message)
    {
        StatusCode = statusCode;
        ApiError = apiError;
    }

    public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;
    public bool IsBadRequest => StatusCode == HttpStatusCode.BadRequest;
    public bool IsConflict => StatusCode == HttpStatusCode.Conflict;
}