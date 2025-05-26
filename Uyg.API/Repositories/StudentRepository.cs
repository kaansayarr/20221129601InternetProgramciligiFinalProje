using Uyg.API.Models;

namespace Uyg.API.Repositories
{
    public class StudentRepository : GenericRepository<Student>
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }
    }
} 