using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.UpdateMovie;

public static class UpdateMovieEndpoint
{
    public static IEndpointConventionBuilder MapUpdateMovieEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapPut("/api/movies/{id:guid}", HandleAsync)
            .WithName("UpdateMovie")
            .WithSummary("Updates a movie")
            .Produces<UpdateMovieResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateMovieRequest request,
        IValidator<UpdateMovieRequest> validator,
        UpdateMovieHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var result = await handler.HandleAsync(id, request, cancellationToken);
        if (result.IsFailure)
        {
            if (result.Error == Error.NotFound)
                return Results.NotFound();

            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [result.Error.Code] = [result.Error.Message]
            });
        }

        return Results.Ok(result.Value);
    }
}
