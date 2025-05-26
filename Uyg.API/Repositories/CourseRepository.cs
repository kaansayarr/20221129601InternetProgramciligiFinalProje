using Uyg.API.Models;

namespace Uyg.API.Repositories
{
    public class CourseRepository : GenericRepository<Course>
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }
    }
} 