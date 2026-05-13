using dance.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace dance.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet для всех 8 моделей
        public DbSet<Client> Clients { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Classes → Trainer (отключаем каскадное удаление)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Trainer)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.Trainer_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Classes → Group (отключаем каскадное удаление)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Group)
                .WithMany(g => g.Classes)
                .HasForeignKey(c => c.Group_id)
                .OnDelete(DeleteBehavior.Restrict);

            // AttendanceRecord → Client
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Client)
                .WithMany(c => c.AttendanceRecords)
                .HasForeignKey(a => a.Client_id)
                .OnDelete(DeleteBehavior.Restrict);

            // AttendanceRecord → Class
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Class)
                .WithMany(c => c.AttendanceRecords)
                .HasForeignKey(a => a.Class_id)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

        }
    }
}
