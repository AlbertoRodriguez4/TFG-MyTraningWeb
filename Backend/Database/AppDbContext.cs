using System.Collections.Generic;
using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using Microsoft.EntityFrameworkCore;
using TaskEntity = AA2_CS.Model.Entities.Task;
namespace AA2_CS.Database;
public class AppDbContext : DbContext
{
    // Contexto de la base de datos que define las tablas y sus relaciones
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserDTO> UserDTOs { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<UserRoom> UserRooms { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<BodyMetric> BodyMetrics { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<TaskExercise> TaskExercises { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapeo de tablas a minúsculas
        modelBuilder.Entity<User>().ToTable("users");

        modelBuilder.Entity<Plan>().ToTable("plans");
        modelBuilder.Entity<Item>().ToTable("items");
        modelBuilder.Entity<Purchase>().ToTable("purchases");
        modelBuilder.Entity<Subscription>().ToTable("subscriptions");
        modelBuilder.Entity<Room>().ToTable("rooms");
        modelBuilder.Entity<UserRoom>().ToTable("usersrooms");
        modelBuilder.Entity<TaskEntity>().ToTable("tasks");
        modelBuilder.Entity<TaskEntity>()
            .Property(t => t.id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Purchase>().ToTable("purchases");
        modelBuilder.Entity<Purchase>()
            .Property(p => p.id)
            .ValueGeneratedOnAdd();

        // Configuración de clave compuesta para la tabla intermedia UserRoom
        modelBuilder.Entity<UserRoom>()
            .HasKey(ur => new { ur.userid, ur.roomid });

        modelBuilder.Entity<UserRoom>()
            .Property(ur => ur.userid).HasColumnName("userid");
        modelBuilder.Entity<UserRoom>()
            .Property(ur => ur.roomid).HasColumnName("roomid");

        // Configurar relaciones sin usar propiedades de navegación
        modelBuilder.Entity<UserRoom>()
            .HasOne(ur => ur.User)  // Relación con User
            .WithMany()  // No necesidad de la propiedad de navegación en User
            .HasForeignKey(ur => ur.userid)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRoom>()
            .HasOne(ur => ur.Room)  // Relación con Room
            .WithMany()  // No necesidad de la propiedad de navegación en Room
            .HasForeignKey(ur => ur.roomid)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración para EmailVerification
        modelBuilder.Entity<EmailVerification>().ToTable("emailverifications");

        // Configuración para NotificationPreference
        modelBuilder.Entity<NotificationPreference>().ToTable("notificationpreferences");
        modelBuilder.Entity<NotificationPreference>()
            .HasIndex(np => np.userid)
            .IsUnique();
        modelBuilder.Entity<NotificationPreference>()
            .HasOne(np => np.User)
            .WithMany()
            .HasForeignKey(np => np.userid)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración para Subscription
        modelBuilder.Entity<Subscription>()
            .HasKey(s => s.id);
        modelBuilder.Entity<Subscription>()
            .Property(s => s.id).HasColumnName("id");
        modelBuilder.Entity<Subscription>()
            .Property(s => s.userid).HasColumnName("userid");
        modelBuilder.Entity<Subscription>()
            .Property(s => s.startDate).HasColumnName("startdate");
        modelBuilder.Entity<Subscription>()
            .Property(s => s.endDate).HasColumnName("enddate");
        modelBuilder.Entity<Subscription>()
            .Property(s => s.isActive).HasColumnName("isactive");
        modelBuilder.Entity<Subscription>()
            .Property(s => s.planType).HasColumnName("planType");
        modelBuilder.Entity<Subscription>()
            .Property(s => s.monthlyPrice).HasColumnName("monthlyPrice");

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.userid)
            .OnDelete(DeleteBehavior.Cascade);

        // Achievements
        modelBuilder.Entity<Achievement>().ToTable("achievements");
        modelBuilder.Entity<UserAchievement>().ToTable("userachievements");
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.User)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.Achievement)
            .WithMany()
            .HasForeignKey(ua => ua.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        // BodyMetrics
        modelBuilder.Entity<BodyMetric>().ToTable("bodymetrics");
        modelBuilder.Entity<BodyMetric>()
            .HasOne(bm => bm.User)
            .WithMany()
            .HasForeignKey(bm => bm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Exercises
        modelBuilder.Entity<Exercise>().ToTable("exercises");

        // TaskExercises
        modelBuilder.Entity<TaskExercise>().ToTable("taskexercises");
        modelBuilder.Entity<TaskExercise>()
            .HasOne(te => te.Task)
            .WithMany()
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TaskExercise>()
            .HasOne(te => te.Exercise)
            .WithMany()
            .HasForeignKey(te => te.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}