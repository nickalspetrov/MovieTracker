using MovieTracker.Application.Common.Repositories;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.DeleteMovie;

public sealed record DeleteMovieRequest(Guid Id);

public sealed class DeleteMovieHandler(IMovieRepository movieRepository)
{
    public async Task<Result> HandleAsync(
        DeleteMovieRequest request,
        CancellationToken cancellationToken)
    {
        return await movieRepository.DeleteAsync(request.Id, cancellationToken)
            ? Result.Success()
            : Result.Failure(Error.NotFound);
    }
}
