using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MovieTracker.Application.Common.Repositories;

namespace MovieTracker.Application.Movies.GetMovieList;

public static class GetMovieDetailsEndpoint
{
    public static IEndpointConventionBuilder MapGetMovieDetailsEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapGet("/api/movies/{id:guid}", async (
            Guid id, IMovieRepository repository, CancellationToken cancellationToken) =>
        {
            var movie = await repository.GetByIdAsync(id, cancellationToken);
            return movie is null ? Results.NotFound() : Results.Ok(movie.ToResponse());
        })
        .WithName("GetMovieDetails")
        .Produces<MovieListItemResponse>()
        .Produces(StatusCodes.Status404NotFound);
}
