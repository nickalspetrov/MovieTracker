using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.UpdateMovie;

public sealed record UpdateMovieRequest(
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);

public sealed record UpdateMovieResponse(
    Guid Id,
    string Title,
    string Description,
    string Genre,
    int ReleaseYear,
    string PosterUrl);

public sealed class UpdateMovieHandler(IMovieRepository movieRepository)
{
    public async Task<Result<UpdateMovieResponse>> HandleAsync(
        Guid id,
        UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        if (await movieRepository.GetByIdAsync(id, cancellationToken) is null)
            return Error.NotFound;

        var creation = request.ToMovie(id);
        if (creation.IsFailure)
            return creation.Error;

        var movie = creation.Value!;
        if (!await movieRepository.UpdateAsync(movie, cancellationToken))
            return Error.NotFound;

        return Result<UpdateMovieResponse>.Success(movie.ToResponse());
    }
}
