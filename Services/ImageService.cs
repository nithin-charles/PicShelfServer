using Microsoft.EntityFrameworkCore;
using PicShelfServer.DbContexts;
using PicShelfServer.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace PicShelfServer.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly PicShelfResourceDbContext dbContext;

        public ImageService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, PicShelfResourceDbContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<Image>> GetAllPics()
        {
            return await dbContext.Images.ToListAsync();
        }

        public async Task<string> GetImageUrl(Guid imageId)
        {
            var image = await dbContext.Images.SingleOrDefaultAsync(image => image.Id.Equals(imageId));

            return image?.FilePath ?? "Image not found";
        }

        public async Task<Image> Upload(Image image)
        {
            var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images",
                $"{image.FileName}{image.FileExtension}");

            // Upload Image to Local Path
            await using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;


            // Add Image to the Images table
            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }

        public async Task<Folder> AddFolder(string folderName)
        {
            if (await dbContext.Folders.AnyAsync(x => x.FolderName == folderName))
            {
                throw new ValidationException("Folder already exists.");
            }
            var folder = new Folder
            {
                FolderName = folderName,
            };
            await this.dbContext.Folders.AddAsync(folder);
            await this.dbContext.SaveChangesAsync();

            return await dbContext.Folders.SingleOrDefaultAsync(x => x.FolderName == folderName);
        }

        public async Task<Folder?> RemoveFolder(string folderName)
        {
            var folder = await  dbContext.Folders.SingleOrDefaultAsync(folder => folder.FolderName == folderName);
            if (folder == null)
            {
                return null;
            }
            this.dbContext.Folders.Remove(folder);
            await this.dbContext.SaveChangesAsync();
            return folder;
        }
    }
}
