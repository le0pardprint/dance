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
    }
}
