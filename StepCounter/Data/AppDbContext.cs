using Microsoft.EntityFrameworkCore;
using StepCounter.Entities.User;

namespace StepCounter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.OwnsOne(x => x.Preferences);
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}