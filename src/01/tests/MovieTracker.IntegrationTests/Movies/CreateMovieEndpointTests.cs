using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieTracker.Application.Movies.CreateMovie;
using MovieTracker.Application.Movies.GetMovieList;

namespace MovieTracker.IntegrationTests.Movies;

[TestFixture]
public sealed class CreateMovieEndpointTests : BaseIntegrationTest
{
    [Test]
    public async Task PostMovie_ReturnsCreatedAndPersistsTheMappedMovie()
    {
        var request = new CreateMovieRequest("  New movie  ", "  Description  ", "drama", 2020, "/poster.svg");
        using var response = await Client.PostAsJsonAsync("/api/movies", request);
        var movie = await response.Content.ReadFromJsonAsync<CreateMovieResponse>();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(movie, Is.Not.Null);
            Assert.That(movie!.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(movie.Title, Is.EqualTo("New movie"));
            Assert.That(movie.Description, Is.EqualTo("Description"));
            Assert.That(movie.Genre, Is.EqualTo("Drama"));
            Assert.That(movie.ReleaseYear, Is.EqualTo(request.ReleaseYear));
            Assert.That(movie.PosterUrl, Is.EqualTo(request.PosterUrl));
            Assert.That(response.Headers.Location?.ToString(), Is.EqualTo($"/api/movies/{movie.Id}"));
        });

        var saved = await Client.GetFromJsonAsync<MovieListItemResponse>(response.Headers.Location);
        Assert.That(saved, Is.EqualTo(new MovieListItemResponse(movie!.Id, movie.Title,
            movie.Description, movie.Genre, movie.ReleaseYear, movie.PosterUrl)));
        var catalogue = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies?pageSize=50");
        Assert.That(catalogue!.Items.Any(item => item.Id == movie.Id), Is.True);
    }

    [Test]
    public async Task PostMovie_WithInvalidFields_ReturnsValidationErrorsWithoutSaving()
    {
        var before = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies");
        var request = new CreateMovieRequest(" ", "", "Unknown", 1887, "");
        using var response = await Client.PostAsJsonAsync("/api/movies", request);
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var errors = payload.RootElement.GetProperty("errors");
        foreach (var field in new[] { "Title", "Description", "Genre", "ReleaseYear", "PosterUrl" })
            Assert.That(errors.TryGetProperty(field, out _), Is.True, field);
        var after = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies");
        Assert.That(after!.TotalCount, Is.EqualTo(before!.TotalCount));
    }
}
