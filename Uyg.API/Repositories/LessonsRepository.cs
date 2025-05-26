using Uyg.API.Models;

namespace Uyg.API.Repositories
{
    public class LessonsRepository : GenericRepository<Lessons>
    {
        public LessonsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
