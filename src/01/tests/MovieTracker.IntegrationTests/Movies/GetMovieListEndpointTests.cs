using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieTracker.Application.Movies.GetMovieList;

namespace MovieTracker.IntegrationTests.Movies;

[TestFixture]
public sealed class GetMovieListEndpointTests : BaseIntegrationTest
{
    [Test]
    public async Task GetMovie_ReturnsTheSameDataAsTheCatalogue()
    {
        var catalogue = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies?pageSize=1");
        var expected = catalogue!.Items.Single();
        var movie = await Client.GetFromJsonAsync<MovieListItemResponse>($"/api/movies/{expected.Id}");
        Assert.That(movie, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetMovie_WithUnknownId_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/movies/{Guid.Empty}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetMovies_ReturnsARequestedPageFromTheInMemoryCatalogue()
    {
        var response = await Client.GetAsync("/api/movies?page=1&pageSize=3");
        var payload = await response.Content.ReadFromJsonAsync<GetMovieListResponse>();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload!.Page, Is.EqualTo(1));
            Assert.That(payload.PageSize, Is.EqualTo(3));
            Assert.That(payload.TotalCount, Is.EqualTo(10));
            Assert.That(
                payload.Items.Select(movie => movie.Title),
                Is.EqualTo(new[] { "Arrival", "Blade Runner 2049", "Inception" }));
            Assert.That(payload.Items.All(movie => movie.PosterUrl.StartsWith("/images/posters/")), Is.True);
        });
    }

    [Test]
    public async Task GetMovies_WithInvalidPagination_ReturnsValidationProblem()
    {
        var response = await Client.GetAsync("/api/movies?page=0&pageSize=51");
        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var errors = payload.RootElement.GetProperty("errors");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(errors.TryGetProperty("Page", out _), Is.True);
            Assert.That(errors.TryGetProperty("PageSize", out _), Is.True);
        });
    }
}
