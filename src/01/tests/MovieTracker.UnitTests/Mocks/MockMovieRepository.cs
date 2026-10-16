using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Movies;

namespace MovieTracker.UnitTests.Mocks;

public sealed class MockMovieRepository(
    int totalCount,
    IReadOnlyList<Movie> movies) : IMovieRepository
{
    public int RequestedSkip { get; private set; }
    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(movies.FirstOrDefault(movie => movie.Id == id));
    public int RequestedTake { get; private set; }

    public Task<bool> AddAsync(Movie movie, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("This mock only supports read operations.");

    public Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("This mock only supports read operations.");

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("This mock only supports read operations.");

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(totalCount);

    public Task<IReadOnlyList<Movie>> GetPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        RequestedSkip = skip;
        RequestedTake = take;
        return Task.FromResult(movies);
    }
}
