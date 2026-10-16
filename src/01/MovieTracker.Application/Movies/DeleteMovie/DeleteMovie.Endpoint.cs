using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MovieTracker.Application.Movies.DeleteMovie;

public static class DeleteMovieEndpoint
{
    public static IEndpointConventionBuilder MapDeleteMovieEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapDelete("/api/movies/{id:guid}", HandleAsync)
            .WithName("DeleteMovie")
            .WithSummary("Deletes a movie")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid id,
        DeleteMovieHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteMovieRequest(id), cancellationToken);
        return result.IsSuccess ? Results.NoContent() : Results.NotFound();
    }
}
