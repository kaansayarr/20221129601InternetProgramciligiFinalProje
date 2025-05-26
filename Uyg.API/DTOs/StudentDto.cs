namespace Uyg.API.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string StudentId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class StudentCreateDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string StudentId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
} 