using Microsoft.EntityFrameworkCore;
using dance.API.Models;

namespace dance.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
            // Связь Client → User (один к одному)
            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Trainer → User (один к одному)
            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.User)
                .WithOne(u => u.Trainer)
                .HasForeignKey<Trainer>(t => t.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            // ← ФИКС: явная связь Trainer → Direction
            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.Direction)
                .WithMany(d => d.Trainers)
                .HasForeignKey(t => t.Direction_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Direction → Group (один ко многим)
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Direction)
                .WithMany(d => d.Groups)
                .HasForeignKey(g => g.Direction_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Trainer → Group (один ко многим)
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Trainer)
                .WithMany(t => t.Groups)
                .HasForeignKey(g => g.Trainer_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Group → Class (один ко многим)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Group)
                .WithMany(g => g.Classes)
                .HasForeignKey(c => c.Group_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Trainer → Class (один ко многим)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Trainer)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.Trainer_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Client → Registration (один ко многим)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Registrations)
                .HasForeignKey(r => r.Client_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Group → Registration (один ко многим)
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Group)
                .WithMany(g => g.Registrations)
                .HasForeignKey(r => r.Group_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Client → Subscription (один ко многим)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Client)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.Client_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Group → Subscription (один ко многим)
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Subscriptions)
                .HasForeignKey(s => s.Group_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Client → AttendanceRecord (один ко многим)
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Client)
                .WithMany(c => c.AttendanceRecords)
                .HasForeignKey(a => a.Client_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь Class → AttendanceRecord (один ко многим)
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(a => a.Class)
                .WithMany(c => c.AttendanceRecords)
                .HasForeignKey(a => a.Class_id)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
