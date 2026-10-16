using System.Collections.Concurrent;
using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Infrastructure.Repositories;

public sealed class InMemoryMovieRepository : IMovieRepository
{
    private static readonly IReadOnlyList<Movie> Movies =
    [
        CreateMovie("00000000-0000-0000-0000-000000000001", "Interstellar", "Explorers cross the stars in search of a future home for humanity.", MovieGenre.ScienceFiction, 2014, "/images/posters/interstellar.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000002", "Inception", "A specialist enters layered dreams to plant an idea in a guarded mind.", MovieGenre.ScienceFiction, 2010, "/images/posters/inception.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000003", "Arrival", "A linguist races to understand mysterious visitors before fear takes over.", MovieGenre.ScienceFiction, 2016, "/images/posters/arrival.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000004", "The Matrix", "A programmer discovers that the familiar world hides a startling truth.", MovieGenre.Action, 1999, "/images/posters/matrix.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000005", "Blade Runner 2049", "A new investigation uncovers a secret that could reshape a divided society.", MovieGenre.ScienceFiction, 2017, "/images/posters/blade-runner-2049.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000006", "Whiplash", "An ambitious drummer is pushed to extremes by a demanding instructor.", MovieGenre.Drama, 2014, "/images/posters/whiplash.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000007", "Parasite", "Two families become entangled through opportunity, deception, and class tension.", MovieGenre.Thriller, 2019, "/images/posters/parasite.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000008", "The Lord of the Rings: The Fellowship of the Ring", "A small fellowship begins a dangerous journey to protect their world.", MovieGenre.Fantasy, 2001, "/images/posters/fellowship.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000009", "Spirited Away", "A young girl enters a magical realm and must find her courage to return home.", MovieGenre.Animation, 2001, "/images/posters/spirited-away.svg"),
        CreateMovie("00000000-0000-0000-0000-000000000010", "The Grand Budapest Hotel", "A devoted concierge and his lobby boy are swept into a comic mystery.", MovieGenre.Comedy, 2014, "/images/posters/grand-budapest.svg")
    ];

    private readonly ConcurrentDictionary<Guid, Movie> movies = new(
        Movies.Select(movie => new KeyValuePair<Guid, Movie>(movie.Id, movie)));

    public Task<bool> AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(movie);
        return Task.FromResult(movies.TryAdd(movie.Id, movie));
    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(movie);
        while (movies.TryGetValue(movie.Id, out var existing))
        {
            if (movies.TryUpdate(movie.Id, movie, existing))
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(movies.TryRemove(id, out _));
    }

    private static Movie CreateMovie(
        string id,
        string title,
        string description,
        MovieGenre genre,
        int releaseYear,
        string posterUrl) =>
        Movie.Create(Guid.Parse(id), title, description, genre, releaseYear, posterUrl).Value!;

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        movies.TryGetValue(id, out var movie);
        return Task.FromResult(movie);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(movies.Count);
    }

    public Task<IReadOnlyList<Movie>> GetPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<Movie> page = movies.Values
            .OrderBy(movie => movie.Title, StringComparer.OrdinalIgnoreCase)
            .Skip(skip)
            .Take(take)
            .ToArray();

        return Task.FromResult(page);
    }
}
