using System.ComponentModel.DataAnnotations;

namespace Uyg.API.Models
{
    public class Student : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Surname { get; set; }
        
        [Required]
        public string StudentId { get; set; }
        
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 