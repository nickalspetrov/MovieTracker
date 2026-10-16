using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieTracker.Application.Movies.GetMovieList;
using MovieTracker.Application.Movies.UpdateMovie;

namespace MovieTracker.IntegrationTests.Movies;

[TestFixture]
public sealed class UpdateMovieEndpointTests : BaseIntegrationTest
{
    private static readonly Guid MovieId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [Test]
    public async Task PutMovie_ReplacesAllFieldsAndPreservesIdAndCount()
    {
        var before = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies");
        var request = new UpdateMovieRequest("  Updated title  ", " Updated description ", "comedy", 2021, " /updated.svg ");
        using var response = await Client.PutAsJsonAsync($"/api/movies/{MovieId}", request);
        var updated = await response.Content.ReadFromJsonAsync<UpdateMovieResponse>();
        var saved = await Client.GetFromJsonAsync<MovieListItemResponse>($"/api/movies/{MovieId}");
        var after = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(updated, Is.EqualTo(new UpdateMovieResponse(MovieId, "Updated title",
                "Updated description", "Comedy", 2021, "/updated.svg")));
            Assert.That(saved, Is.EqualTo(new MovieListItemResponse(MovieId, "Updated title",
                "Updated description", "Comedy", 2021, "/updated.svg")));
            Assert.That(after!.TotalCount, Is.EqualTo(before!.TotalCount));
        });
    }

    [Test]
    public async Task PutMovie_WithUnknownId_ReturnsNotFoundWithoutCreatingMovie()
    {
        var id = Guid.NewGuid();
        using var response = await Client.PutAsJsonAsync($"/api/movies/{id}",
            new UpdateMovieRequest("Title", "Description", "Drama", 2020, "/poster.svg"));
        using var lookup = await Client.GetAsync($"/api/movies/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(lookup.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PutMovie_WithInvalidFields_ReturnsErrorsAndPreservesMovie()
    {
        var before = await Client.GetFromJsonAsync<MovieListItemResponse>($"/api/movies/{MovieId}");
        using var response = await Client.PutAsJsonAsync($"/api/movies/{MovieId}",
            new UpdateMovieRequest(" ", "", "Unknown", 1887, ""));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var errors = payload.RootElement.GetProperty("errors");
        foreach (var field in new[] { "Title", "Description", "Genre", "ReleaseYear", "PosterUrl" })
            Assert.That(errors.TryGetProperty(field, out _), Is.True, field);
        var after = await Client.GetFromJsonAsync<MovieListItemResponse>($"/api/movies/{MovieId}");
        Assert.That(after, Is.EqualTo(before));
    }
}
