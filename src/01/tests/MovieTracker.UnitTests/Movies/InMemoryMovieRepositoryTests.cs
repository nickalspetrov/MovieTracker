using MovieTracker.Domain.Movies;
using MovieTracker.Infrastructure.Repositories;

namespace MovieTracker.UnitTests.Movies;

[TestFixture]
public sealed class InMemoryMovieRepositoryTests
{
    private static Movie CreateMovie(Guid id, string title) =>
        Movie.Create(id, title, "Description", MovieGenre.Drama, 2020, "/poster.svg").Value!;

    [Test]
    public async Task Movie_CanBeCreatedReadUpdatedAndDeleted()
    {
        var repository = new InMemoryMovieRepository();
        var initialCount = await repository.CountAsync();
        var movie = CreateMovie(Guid.NewGuid(), "A new movie");

        Assert.That(await repository.AddAsync(movie), Is.True);
        Assert.That(await repository.GetByIdAsync(movie.Id), Is.SameAs(movie));
        Assert.That(await repository.CountAsync(), Is.EqualTo(initialCount + 1));
        Assert.That(await repository.AddAsync(CreateMovie(movie.Id, "Duplicate")), Is.False);

        var updated = CreateMovie(movie.Id, "An updated movie");
        Assert.That(await repository.UpdateAsync(updated), Is.True);
        Assert.That(await repository.GetByIdAsync(movie.Id), Is.SameAs(updated));
        var page = await repository.GetPagedAsync(0, 50);
        Assert.That(page.Single(item => item.Id == movie.Id).Title, Is.EqualTo(updated.Title));

        Assert.That(await repository.DeleteAsync(movie.Id), Is.True);
        Assert.That(await repository.GetByIdAsync(movie.Id), Is.Null);
        Assert.That(await repository.CountAsync(), Is.EqualTo(initialCount));
        Assert.That(await repository.UpdateAsync(updated), Is.False);
        Assert.That(await repository.DeleteAsync(movie.Id), Is.False);
    }

    [Test]
    public async Task Changes_AreIsolatedBetweenRepositoryInstances()
    {
        var first = new InMemoryMovieRepository();
        var second = new InMemoryMovieRepository();
        var movie = CreateMovie(Guid.NewGuid(), "Independent movie");
        await first.AddAsync(movie);
        Assert.That(await second.GetByIdAsync(movie.Id), Is.Null);
        var seededMovie = (await first.GetPagedAsync(0, 1)).Single();
        await first.DeleteAsync(seededMovie.Id);
        Assert.That(await second.GetByIdAsync(seededMovie.Id), Is.Not.Null);
    }

    [Test]
    public void CancelledWrites_DoNotChangeTheCatalogue()
    {
        var repository = new InMemoryMovieRepository();
        var movie = CreateMovie(Guid.NewGuid(), "Cancelled movie");
        var token = new CancellationToken(true);
        Assert.ThrowsAsync<OperationCanceledException>(async () => await repository.AddAsync(movie, token));
        Assert.ThrowsAsync<OperationCanceledException>(async () => await repository.UpdateAsync(movie, token));
        Assert.ThrowsAsync<OperationCanceledException>(async () => await repository.DeleteAsync(movie.Id, token));
    }
}
