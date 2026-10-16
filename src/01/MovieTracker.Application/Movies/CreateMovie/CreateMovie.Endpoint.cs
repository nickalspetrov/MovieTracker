using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MovieTracker.Domain.Common.Results;

namespace MovieTracker.Application.Movies.CreateMovie;

public static class CreateMovieEndpoint
{
    public static IEndpointConventionBuilder MapCreateMovieEndpoint(this IEndpointRouteBuilder endpoints)
        => endpoints.MapPost("/api/movies", HandleAsync)
            .WithName("CreateMovie")
            .WithSummary("Creates a movie")
            .Produces<CreateMovieResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);

    private static async Task<IResult> HandleAsync(
        CreateMovieRequest request,
        IValidator<CreateMovieRequest> validator,
        CreateMovieHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var result = await handler.HandleAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            if (result.Error == Error.Exist)
                return Results.Conflict(result.Error);

            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [result.Error.Code] = [result.Error.Message]
            });
        }

        var movie = result.Value!;
        return Results.Created($"/api/movies/{movie.Id}", movie);
    }
}
