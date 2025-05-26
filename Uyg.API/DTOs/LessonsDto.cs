namespace Uyg.API.DTOs
{
    public class LessonsDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string PhotoUrl { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public CategoryDto? Category { get; set; }
    }
}
