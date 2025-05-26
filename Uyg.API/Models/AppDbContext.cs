using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Uyg.API.Models
{
    public class AppDbContext:IdentityDbContext<AppUser,AppRole,string>
    {
      public DbSet<Lessons> Lessons { get; set; }
      public DbSet<Category> Categories { get; set; }
      public DbSet<Course> Courses { get; set; }
      public DbSet<Student> Students { get; set; }
      public DbSet<Enrollment> Enrollments { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Lessons>()
                .HasOne(l => l.Category)
                .WithMany()
                .HasForeignKey(l => l.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lessons>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentDbId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Lessons)
                .WithMany()
                .HasForeignKey(e => e.LessonsId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
