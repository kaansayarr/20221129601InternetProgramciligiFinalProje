namespace Uyg.API.DTOs
{
    public class CourseDto : BaseDto
    {
        public CourseDto()
        {
            Lessons = new List<CourseLessonDto>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public List<CourseLessonDto> Lessons { get; set; }
    }

    public class CourseLessonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string PhotoUrl { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
    }
} 