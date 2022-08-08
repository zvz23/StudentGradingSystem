using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data {

    public class GradingContext : DbContext {

        public GradingContext(DbContextOptions options):
        base(options) {

        }
        public DbSet<StudentAccount>? Students { get; set; }
        public DbSet<Subject>? Subjects { get; set; }
        public DbSet<SchoolProgram>? Programs { get; set; }
        public DbSet<Prerequisite>? Prerequisites { get; set; }
        public DbSet<ProgramHead>? ProgramHeads { get; set; }
        public DbSet<Administrator>? Administrators { get; set; }
        public DbSet<SubjectGrade>? SubjectGrades { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            
            modelBuilder.Entity<Prerequisite>()
                .Property(p => p.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Subject>()
                .Property(s => s.ClassType)
                .HasConversion<string>();

            modelBuilder.Entity<Subject>()
                .Property(s => s.Year)
                .HasConversion<string>();
            
            modelBuilder.Entity<Subject>()
                .Property(s => s.Semester)
                .HasConversion<string>();

        }
    }
}