using FluentValidation;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.UpdateMovie;

public sealed class UpdateMovieValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieValidator()
    {
        RuleFor(request => request.Title).NotEmpty();
        RuleFor(request => request.Description).NotEmpty();
        RuleFor(request => request.Genre)
            .Must(value => Enum.TryParse<MovieGenre>(value, true, out var genre) && Enum.IsDefined(genre))
            .WithMessage("A valid movie genre is required.");
        RuleFor(request => request.ReleaseYear).GreaterThanOrEqualTo(1888);
        RuleFor(request => request.PosterUrl).NotEmpty();
    }
}
