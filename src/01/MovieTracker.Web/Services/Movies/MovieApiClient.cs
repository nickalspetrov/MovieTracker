using System.Net.Http.Json;
using MovieTracker.Web.Models.Movies;

namespace MovieTracker.Web.Services.Movies;

public sealed class MovieApiClient(HttpClient httpClient)
{
    public async Task<MovieListItemModel> CreateMovieAsync(MovieFormModel movie, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/movies", movie, cancellationToken);
        await EnsureWriteSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<MovieListItemModel>(cancellationToken)
            ?? throw new InvalidOperationException("The movie API returned an empty response.");
    }

    public async Task<MovieListItemModel> UpdateMovieAsync(Guid id, MovieFormModel movie, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"api/movies/{id}", movie, cancellationToken);
        await EnsureWriteSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<MovieListItemModel>(cancellationToken)
            ?? throw new InvalidOperationException("The movie API returned an empty response.");
    }

    public async Task DeleteMovieAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/movies/{id}", cancellationToken);
        await EnsureWriteSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureWriteSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        Dictionary<string, string[]>? errors = null;
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            try
            {
                using var payload = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
                if (payload.RootElement.TryGetProperty("errors", out var fields))
                    errors = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(fields.GetRawText());
            }
            catch (System.Text.Json.JsonException) { }
        }

        var message = response.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => "This movie no longer exists. Return to the catalogue to continue.",
            System.Net.HttpStatusCode.BadRequest => "Please check the movie details and try again.",
            System.Net.HttpStatusCode.Conflict => "The movie could not be saved because it conflicts with an existing movie.",
            _ => "The movie service could not complete the request. Please try again."
        };
        throw new MovieApiException(message, response.StatusCode, errors);
    }

    public async Task<MovieListItemModel?> GetMovieAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/movies/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieListItemModel>(cancellationToken)
            ?? throw new InvalidOperationException("The movie API returned an empty response.");
    }

    public async Task<MovieListResponseModel> GetMoviesAsync(int page = 1, int pageSize = 24, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/movies?page={page}&pageSize={pageSize}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieListResponseModel>(cancellationToken)
            ?? throw new InvalidOperationException("The movie API returned an empty response.");
    }
}
