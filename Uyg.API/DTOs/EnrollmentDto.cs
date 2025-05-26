using System.ComponentModel.DataAnnotations;

namespace Uyg.API.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentDbId { get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
        public string StudentId { get; set; }
        public int LessonsId { get; set; }
        public string LessonsName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class EnrollmentCreateDto
    {
        [Required(ErrorMessage = "Öğrenci ID'si zorunludur")]
        [Display(Name = "Öğrenci ID")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Ders ID'si zorunludur")]
        [Display(Name = "Ders ID")]
        public int LessonsId { get; set; }
    }
} 