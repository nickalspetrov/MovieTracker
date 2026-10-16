using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.CreateMovie;

internal static class CreateMovieMapping
{
    internal static Result<Movie> ToMovie(this CreateMovieRequest request)
    {
        if (!Enum.TryParse<MovieGenre>(request.Genre, true, out var genre) || !Enum.IsDefined(genre))
            return new Error("Movies.InvalidGenre", "A valid movie genre is required.");

        return Movie.Create(Guid.NewGuid(), request.Title, request.Description,
            genre, request.ReleaseYear, request.PosterUrl);
    }

    internal static CreateMovieResponse ToResponse(this Movie movie) =>
        new(movie.Id, movie.Title, movie.Description, movie.Genre.ToString(),
            movie.ReleaseYear, movie.PosterUrl);
}
