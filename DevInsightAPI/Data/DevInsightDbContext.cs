using DevInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DevInsightAPI.Data
{
    public class DevInsightDbContext : DbContext
    {
        public DevInsightDbContext(DbContextOptions<DevInsightDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Project> Projects => Set<Project>();

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(user => user.Name)
                    .HasMaxLength(120);

                entity.Property(user => user.Email)
                    .HasMaxLength(180);

                entity.Property(user => user.Role)
                    .HasMaxLength(60);

                entity.Property(user => user.PasswordHash)
                    .HasMaxLength(400);

                entity.HasIndex(user => user.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(project => project.Name)
                    .HasMaxLength(140);

                entity.Property(project => project.Description)
                    .HasMaxLength(600);

                entity.HasOne(project => project.CreatedByUser)
                    .WithMany(user => user.CreatedProjects)
                    .HasForeignKey(project => project.CreatedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.Property(task => task.Title)
                    .HasMaxLength(160);

                entity.Property(task => task.Description)
                    .HasMaxLength(1200);

                entity.Property(task => task.Status)
                    .HasMaxLength(40);

                entity.Property(task => task.Priority)
                    .HasMaxLength(40);

                entity.HasOne(task => task.Project)
                    .WithMany(project => project.Tasks)
                    .HasForeignKey(task => task.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(task => task.AssignedUser)
                    .WithMany(user => user.AssignedTasks)
                    .HasForeignKey(task => task.AssignedUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(notification => notification.Message)
                    .HasMaxLength(500);

                entity.HasOne(notification => notification.User)
                    .WithMany(user => user.Notifications)
                    .HasForeignKey(notification => notification.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
