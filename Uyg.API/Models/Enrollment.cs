using System.ComponentModel.DataAnnotations;

namespace Uyg.API.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        [Required]
        public int StudentDbId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int LessonsId { get; set; }
        public Lessons Lessons { get; set; }

        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 