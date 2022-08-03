using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data {

    public class GradingContext : DbContext {

        public GradingContext(DbContextOptions options):
        base(options) {

        }
        public DbSet<StudentAccount>? StudentAccounts { get; set; }
        public DbSet<Subject>? Subjects { get; set; }
        public DbSet<Models.Program>? Programs { get; set; }
        public DbSet<Prerequisite>? Prerequisites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Models.Program>()
                .HasIndex(p => p.ProgramName)
                .IsUnique();
            
            modelBuilder.Entity<Prerequisite>()
                .Property(p => p.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Subject>()
                .Property(s => s.Type)
                .HasConversion<string>();
        }
    }
}