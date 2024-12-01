using Microsoft.EntityFrameworkCore;
using PicShelfServer.Models.Domain;

namespace PicShelfServer.DbContexts
{
    public class PicShelfResourceDbContext: DbContext
    {
        public PicShelfResourceDbContext(DbContextOptions<PicShelfResourceDbContext> dbContextOptions): base(dbContextOptions)
        {
            
        }
        public DbSet<Image> Images { get; set; }
    }
}
