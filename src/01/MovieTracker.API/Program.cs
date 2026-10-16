using FluentValidation;
using MovieTracker.Application.Common.Repositories;
using MovieTracker.Application.Movies.CreateMovie;
using MovieTracker.Application.Movies.DeleteMovie;
using MovieTracker.Application.Movies.GetMovieList;
using MovieTracker.Application.Movies.UpdateMovie;
using MovieTracker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMovieRepository, InMemoryMovieRepository>();
builder.Services.AddScoped<GetMovieListHandler>();
builder.Services.AddScoped<CreateMovieHandler>();
builder.Services.AddScoped<DeleteMovieHandler>();
builder.Services.AddScoped<UpdateMovieHandler>();
builder.Services.AddScoped<IValidator<UpdateMovieRequest>, UpdateMovieValidator>();
builder.Services.AddScoped<IValidator<CreateMovieRequest>, CreateMovieValidator>();
builder.Services.AddScoped<IValidator<GetMovieListRequest>, GetMovieListValidator>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("MovieTrackerWeb", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("MovieTrackerWeb");
app.MapGetMovieListEndpoint();
app.MapCreateMovieEndpoint();
app.MapDeleteMovieEndpoint();
app.MapUpdateMovieEndpoint();
app.MapGetMovieDetailsEndpoint();

app.Run();

public partial class Program;
