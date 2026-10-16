using System.ComponentModel.DataAnnotations;

namespace MovieTracker.Web.Models.Movies;

public sealed class MovieFormModel
{
    public static IReadOnlyList<string> Genres { get; } =
        ["Action", "Drama", "Comedy", "ScienceFiction", "Thriller", "Animation", "Crime", "Fantasy"];

    [Required] public string Title { get; set; } = "";
    [Required] public string Description { get; set; } = "";
    [Required] public string Genre { get; set; } = "Drama";
    [Range(1888, int.MaxValue)] public int ReleaseYear { get; set; } = DateTime.Today.Year;
    [Required] public string PosterUrl { get; set; } = "";
}
