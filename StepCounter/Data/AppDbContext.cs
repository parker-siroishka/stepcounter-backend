using Microsoft.EntityFrameworkCore;
using StepCounter.Entities;
using StepCounter.Entities.Routes;
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
        public DbSet<Checkpoint> Checkpoints { get; set; }
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
                .HasMany(r => r.RoutesProgress)
                .WithOne(urp => urp.Route)
                .HasForeignKey(urp => urp.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
            // Route-Checkpoint
            modelBuilder.Entity<Route>()
                .HasMany(r => r.Checkpoints)
                .WithOne(c => c.Route)
                .HasForeignKey(c => c.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Geometry Configurations
            modelBuilder.Entity<Checkpoint>()
                .Property(c => c.Location)
                .HasColumnType("geometry(Point, 4326)");
            modelBuilder.Entity<Route>()
                .Property(r => r.RouteGeometry)
                .HasColumnType("geometry(LineString, 4326)");
            
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.DetectChanges();
            foreach (var entity in ChangeTracker.Entries())
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    entity.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
                else if (entity.State == EntityState.Modified)
                {
                    entity.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}