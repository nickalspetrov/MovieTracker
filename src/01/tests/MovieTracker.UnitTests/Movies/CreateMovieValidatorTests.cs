using MovieTracker.Application.Movies.CreateMovie;

namespace MovieTracker.UnitTests.Movies;

[TestFixture]
public sealed class CreateMovieValidatorTests
{
    [TestCase("Drama", true)]
    [TestCase("sciencefiction", true)]
    [TestCase("Unknown", false)]
    [TestCase("999", false)]
    [TestCase(null, false)]
    public async Task Genre_MustRepresentADefinedGenre(string? genre, bool valid)
    {
        var request = new CreateMovieRequest("Title", "Description", genre!, 1888, "/poster.svg");
        var result = await new CreateMovieValidator().ValidateAsync(request);
        Assert.That(result.IsValid, Is.EqualTo(valid));
    }
}
