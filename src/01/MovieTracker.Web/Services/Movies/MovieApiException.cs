namespace MovieTracker.Web.Services.Movies;

public sealed class MovieApiException(
    string message,
    System.Net.HttpStatusCode statusCode,
    Dictionary<string, string[]>? errors = null) : Exception(message)
{
    public System.Net.HttpStatusCode StatusCode { get; } = statusCode;
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors ?? [];
}
