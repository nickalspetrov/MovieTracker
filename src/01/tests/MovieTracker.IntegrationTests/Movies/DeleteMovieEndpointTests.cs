using System.Net;
using System.Net.Http.Json;
using MovieTracker.Application.Movies.GetMovieList;

namespace MovieTracker.IntegrationTests.Movies;

[TestFixture]
public sealed class DeleteMovieEndpointTests : BaseIntegrationTest
{
    [Test]
    public async Task DeleteMovie_RemovesMovieFromDetailsAndCatalogue()
    {
        var before = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies?pageSize=50");
        var id = before!.Items.First().Id;

        using var response = await Client.DeleteAsync($"/api/movies/{id}");
        using var lookup = await Client.GetAsync($"/api/movies/{id}");
        var after = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies?pageSize=50");
        using var repeated = await Client.DeleteAsync($"/api/movies/{id}");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(lookup.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(after!.TotalCount, Is.EqualTo(before.TotalCount - 1));
            Assert.That(after.Items.Any(movie => movie.Id == id), Is.False);
            Assert.That(repeated.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
        Assert.That(await response.Content.ReadAsStringAsync(), Is.Empty);
    }

    [Test]
    public async Task DeleteMovie_WithUnknownId_ReturnsNotFoundWithoutChangingCount()
    {
        var before = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies");
        using var response = await Client.DeleteAsync($"/api/movies/{Guid.NewGuid()}");
        var after = await Client.GetFromJsonAsync<GetMovieListResponse>("/api/movies");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(after!.TotalCount, Is.EqualTo(before!.TotalCount));
    }
}
