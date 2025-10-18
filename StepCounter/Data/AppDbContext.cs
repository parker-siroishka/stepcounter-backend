using Microsoft.EntityFrameworkCore;
using StepCounter.Entities;
using StepCounter.Entities.Step;
using StepCounter.Entities.Users;
using Route = StepCounter.Entities.Routes.Route;

namespace StepCounter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<UserRouteProgress> UserRouteProgress { get; set; }
        public DbSet<StepRecord> StepRecords { get; set; }
            
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User-owned preferences
            modelBuilder.Entity<User>().OwnsOne(u => u.Preferences);
            // User-StepRecord
            modelBuilder.Entity<User>()
                .HasMany(u => u.StepRecords)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // User-UserRouteProgress
            modelBuilder.Entity<User>()
                .HasMany(u => u.RoutesProgress)
                .WithOne(urp => urp.User)
                .HasForeignKey(urp => urp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // User-Route
            modelBuilder.Entity<User>()
                .HasMany(u => u.Routes)
                .WithOne(r => r.Creator)
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.SetNull);
            // Route-UserRouteProgress
            modelBuilder.Entity<Route>()
                .HasOne(r => r.RouteProgress)
                .WithOne(urp => urp.Route)
                .HasForeignKey<UserRouteProgress>(urp => urp.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
            // Geometry Configurations
            modelBuilder.Entity<Route>()
                .Property(r => r.RouteGeometry)
                .HasColumnType("geometry(LineString, 4326)");
            
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.DetectChanges();
            // Set CreatedAt and UpdatedAt values for all entities
            foreach (var entity in ChangeTracker.Entries())
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.Property("CreatedAt").CurrentValue = DateTimeOffset.UtcNow;
                        entity.Property("UpdatedAt").CurrentValue = DateTimeOffset.UtcNow;
                        break;
                    case EntityState.Modified:
                        entity.Property("UpdatedAt").CurrentValue = DateTimeOffset.UtcNow;
                        break;
                }
            }
            
            var newOrUpdatedRoutes = ChangeTracker.Entries<Route>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToList();
            
            // Calculate route total distance on new or updated routes
            foreach (var route in newOrUpdatedRoutes)
            {
                await Database.ExecuteSqlRawAsync(
                    @"UPDATE ""Routes""
                      SET ""TotalDistance"" = ST_Length(""RouteGeometry""::geography)
                      WHERE ""Id"" = @p0;",
                    parameters: [route.Id]);

            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}