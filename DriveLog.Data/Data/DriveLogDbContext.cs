using DriveLog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DriveLog.Data.Data;

public class DriveLogDbContext : DbContext {
    public DriveLogDbContext(DbContextOptions<DriveLogDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<DriverDocument> DriveDocuments { get; set; }
    public DbSet<DriveLogEntry> DriveLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Vehicle>().ToTable("Vehicles");
        modelBuilder.Entity<DriverDocument>().ToTable("DriverDocuments");
        modelBuilder.Entity<DriveLogEntry>().ToTable("DriveLogs");

        modelBuilder.Entity<DriverDocument>().HasOne<User>().WithMany().HasForeignKey(d => d.UserId);
        modelBuilder.Entity<DriveLogEntry>().HasOne<User>().WithMany().HasForeignKey(d => d.UserId);
        modelBuilder.Entity<DriveLogEntry>().HasOne<Vehicle>().WithMany().HasForeignKey(d => d.VehicleId);
    }
}