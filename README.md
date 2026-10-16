# MovieTracker

The Blazor frontend uses the ASP.NET Core API for the movie catalogue and movie details. Data is currently stored in memory. Account, watchlist, rating, and statistics screens are demos and do not persist changes.

Requires the .NET 10 SDK. Run both projects in separate terminals from the repository root:

```powershell
dotnet run --project src/01/MovieTracker.API --launch-profile https
dotnet run --project src/01/MovieTracker.Web --launch-profile https
```

Open https://localhost:7002/movies. Trust the local HTTPS certificate with `dotnet dev-certs https --trust` if needed.

The frontend API address is configured in `src/01/MovieTracker.Web/wwwroot/appsettings.json`. Its HTTPS address matches the API HTTPS launch profile. Both projects must be running. API CORS origins are configured in `MovieTracker.API/appsettings.Development.json`; update those and the frontend address when changing ports or deploying.

Endpoints:

- `GET /api/movies?page=1&pageSize=24`
- `GET /api/movies/{id}` (returns 404 for an unknown movie)

Run verification with `dotnet test src/01/MovieTracker.slnx`.
