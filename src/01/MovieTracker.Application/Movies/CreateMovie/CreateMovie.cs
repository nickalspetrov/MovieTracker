using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.CreateMovie;

public sealed record CreateMovieRequest(
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);

public sealed record CreateMovieResponse(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);

public sealed class CreateMovieHandler(IMovieRepository movieRepository)
{
    public async Task<Result<CreateMovieResponse>> HandleAsync(
        CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var creation = request.ToMovie();
        if (creation.IsFailure)
            return creation.Error;

        var movie = creation.Value!;
        if (!await movieRepository.AddAsync(movie, cancellationToken))
            return Error.Exist;

        return Result<CreateMovieResponse>.Success(movie.ToResponse());
    }
}
