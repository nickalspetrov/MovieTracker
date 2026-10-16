using MovieTracker.Application.Movies.CreateMovie;
using MovieTracker.Application.Movies.UpdateMovie;

namespace MovieTracker.UnitTests.Movies;

[TestFixture]
public sealed class UpdateMovieValidatorTests
{
    [TestCase("Title", "Description", "drama", 1888, "/poster.svg", true)]
    [TestCase(" ", "Description", "Drama", 2020, "/poster.svg", false)]
    [TestCase("Title", null, "Drama", 2020, "/poster.svg", false)]
    [TestCase("Title", "Description", "999", 2020, "/poster.svg", false)]
    [TestCase("Title", "Description", null, 2020, "/poster.svg", false)]
    [TestCase("Title", "Description", "Drama", 1887, "/poster.svg", false)]
    [TestCase("Title", "Description", "Drama", 2020, " ", false)]
    public async Task Validation_MatchesCreate(string title, string? description, string? genre,
        int year, string poster, bool valid)
    {
        var update = await new UpdateMovieValidator().ValidateAsync(
            new UpdateMovieRequest(title, description!, genre!, year, poster));
        var create = await new CreateMovieValidator().ValidateAsync(
            new CreateMovieRequest(title, description!, genre!, year, poster));
        Assert.That(update.IsValid, Is.EqualTo(valid));
        Assert.That(update.Errors.Select(error => (error.PropertyName, error.ErrorMessage)),
            Is.EqualTo(create.Errors.Select(error => (error.PropertyName, error.ErrorMessage))));
    }
}
