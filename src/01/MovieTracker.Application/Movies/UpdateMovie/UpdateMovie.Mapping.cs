using MovieTracker.Domain.Common.Results;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.UpdateMovie;

internal static class UpdateMovieMapping
{
    internal static Result<Movie> ToMovie(this UpdateMovieRequest request, Guid id)
    {
        if (!Enum.TryParse<MovieGenre>(request.Genre, true, out var genre) || !Enum.IsDefined(genre))
            return new Error("Movies.InvalidGenre", "A valid movie genre is required.");

        return Movie.Create(id, request.Title, request.Description,
            genre, request.ReleaseYear, request.PosterUrl);
    }

    internal static UpdateMovieResponse ToResponse(this Movie movie) =>
        new(movie.Id, movie.Title, movie.Description, movie.Genre.ToString(),
            movie.ReleaseYear, movie.PosterUrl);
}
