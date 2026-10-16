using FluentValidation;
using MovieTracker.Domain.Movies;

namespace MovieTracker.Application.Movies.CreateMovie;

public sealed class CreateMovieValidator : AbstractValidator<CreateMovieRequest>
{
    public CreateMovieValidator()
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
