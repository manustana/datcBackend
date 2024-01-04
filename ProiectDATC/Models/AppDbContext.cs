// Models/UserDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProiectDATC.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "NORMAL" },
            new Role { Id = 2, Name = "ADMIN" }
        );

        modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
