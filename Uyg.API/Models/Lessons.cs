namespace Uyg.API.Models
{
    public class Lessons:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public string VideoUrl { get; set; }
        public string PhotoUrl { get; set; }
        public int CourseId { get; set; }
        public Category? Category { get; set; }  
        public Course? Course { get; set; }
    }
}
