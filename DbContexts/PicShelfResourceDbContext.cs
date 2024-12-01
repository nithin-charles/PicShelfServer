using Microsoft.AspNetCore.Identity;
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
        public DbSet<Folder> Folders { get; set; }

        /// <summary>
        /// Seeding Other Folder
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            const string folderName = "Others";

            var folder = new Folder {
                Id = new Guid("E45FD846-F89B-44F9-9372-FC24C3395CEA"),
                FolderName = folderName };

            builder.Entity<Folder>().HasData(folder);
        }
    }
}
