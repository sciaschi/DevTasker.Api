using DevTasker.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevTasker.Infrastructure
{
    public class DevTaskerDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }

        public DevTaskerDbContext() {}

        public DevTaskerDbContext(DbContextOptions options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasMany(x => x.TaskItems)
                .WithOne()
                .HasForeignKey(x => x.ProjectId);

            modelBuilder.Entity<TaskItem>()
                .HasMany(x => x.WorkLogs)
                .WithOne()
                .HasForeignKey(x => x.TaskItemId);

            modelBuilder.Entity<Project>()
                .Property(p => p.CreatedAt)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedAt)
                .ValueGeneratedOnAdd();
        }
    }
}
