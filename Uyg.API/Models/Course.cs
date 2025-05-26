namespace Uyg.API.Models
{
    public class Course : BaseEntity
    {
        public Course()
        {
            Lessons = new List<Lessons>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Lessons> Lessons { get; set; }
    }
} 