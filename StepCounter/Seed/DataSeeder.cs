using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using StepCounter.Data;
using StepCounter.Entities;
using StepCounter.Entities.Routes;
using StepCounter.Entities.Step;
using StepCounter.Entities.Users;
using Route = StepCounter.Entities.Routes.Route;

namespace StepCounter.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }
        
        // USERS
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "Parker Siroishka",
            Email = "test@test.com",
            // TODO: Add password hashing for known password
            PasswordHash = "hashed_password",
            AvatarUrl = "https://fastly.picsum.photos/id/823/200/300.jpg?hmac=Sv69FIuXkj79IVp4uZ1YpgRHDGP0jadf5nSiTx1xSoo",
            Role = "User",
            Preferences = new UserPreferences()
        };
        dbContext.Users.Add(user);
        
        // ROUTES
        var route1 = new Route
        {
            Id = Guid.NewGuid(),
            Creator = user,
            Title = "Brisbane River Walk",
            Description = "A scenic  riverside route in Brisbane",
            RouteGeometry = new LineString(new[]
            {
                new Coordinate(153.0281, -27.4698),
                new Coordinate(153.0305, -27.4701),
                new Coordinate(153.0332, -27.4710),
                new Coordinate(153.0357, -27.4720)
            }) { SRID = 4326 }
        };
        
        var route2 = new Route
        {
            Id = Guid.NewGuid(),
            Creator = user,
            Title = "South Bank Loop",
            Description = "An easy loop around South Bank Parklands.",
            RouteGeometry = new LineString(new[]
            {
                new Coordinate(153.0199, -27.4801),
                new Coordinate(153.0210, -27.4815),
                new Coordinate(153.0225, -27.4828),
                new Coordinate(153.0235, -27.4810),
                new Coordinate(153.0199, -27.4801)
            }) { SRID = 4326 }
        };
        dbContext.Routes.AddRange(route1, route2);
        
        // CHECKPOINTS
        var checkpoints = new List<Checkpoint>
        {
            new() { Id = Guid.NewGuid(), Route = route1, Order = 1, Location = new Point(153.0281, -27.4698) { SRID = 4326 } },
            new() { Id = Guid.NewGuid(), Route = route1, Order = 2, Location = new Point(153.0332, -27.4710) { SRID = 4326 } },
            new() { Id = Guid.NewGuid(), Route = route2, Order = 1, Location = new Point(153.0210, -27.4815) { SRID = 4326 } },
            new() { Id = Guid.NewGuid(), Route = route2, Order = 2, Location = new Point(153.0235, -27.4810) { SRID = 4326 } }
        };
        dbContext.Checkpoints.AddRange(checkpoints);
        
        // --- USER ROUTE PROGRESS ---
        var userProgress1 = new UserRouteProgress
        {
            Id = Guid.NewGuid(),
            User = user,
            Route = route1,
            DistanceTravelled = 0.0,
            StartedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var userProgress2 = new UserRouteProgress
        {
            Id = Guid.NewGuid(),
            User = user,
            Route = route2,
            DistanceTravelled = 1.2,
            StartedAt = DateTimeOffset.UtcNow.AddHours(-5)
        };
        dbContext.UserRouteProgress.AddRange(userProgress1, userProgress2);
        
        // --- STEP RECORDS ---
        var steps = new List<StepRecord>
        {
            new()
            {
                Id = Guid.NewGuid(),
                User = user,
                StartTime = DateTimeOffset.UtcNow.AddHours(-2),
                EndTime = DateTimeOffset.UtcNow.AddHours(-1),
                Steps = 4500,
                DistanceTravelled = 3.1,
                Source = "HealthKit",
                SourceId = Guid.NewGuid().ToString(),
                Processed = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                User = user,
                StartTime = DateTimeOffset.UtcNow.AddHours(-1),
                EndTime = DateTimeOffset.UtcNow,
                Steps = 2200,
                DistanceTravelled = 1.4,
                Source = "HealthKit",
                SourceId = Guid.NewGuid().ToString(),
                Processed = false
            }
        };
        dbContext.StepRecords.AddRange(steps);

        await dbContext.SaveChangesAsync();
        
        await dbContext.Database.ExecuteSqlRawAsync(
            @"UPDATE ""Routes"" 
              SET ""TotalDistance"" = ST_Length(""RouteGeometry""::geography)
              WHERE ""RouteGeometry"" IS NOT NULL;");
    }
}