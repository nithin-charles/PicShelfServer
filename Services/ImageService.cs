using Microsoft.EntityFrameworkCore;
using PicShelfServer.DbContexts;
using PicShelfServer.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace PicShelfServer.Services
{
    public class ImageService : IImageService
    {
        private readonly string _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
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


        public async Task<IReadOnlyCollection<Image>> GetPicByFolder(string folderName)
        {
            return await dbContext.Images.Where(img => img.FolderName == folderName).ToListAsync();
        }

        public void DeletePic(Guid id)
        {
            var img = dbContext.Images.FirstOrDefault(img => img.Id == id);
            if(img != null)
            {
                var imageName= img.FileName;
                var filePath = Path.Combine(_imageFolderPath, imageName);
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    throw new ValidationException($"Internal server error: {ex.Message}");
                }
                dbContext.Images.Remove(img);
                dbContext.SaveChanges();
            }
        }

        public async Task<Image> MoveToFolder(Guid imageId, string folderName)
        {
            var image = await dbContext.Images.SingleOrDefaultAsync(image => image.Id.Equals(imageId));
            if (image == null) return null;
            var existingFolder = await dbContext.Folders.SingleOrDefaultAsync(folder => folder.FolderName.Equals(folderName));
            if(existingFolder == null)
            {
                throw new ArgumentException("No such folder name");
            }
            image.FolderName = folderName;
            await dbContext.SaveChangesAsync();
            return image;
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

        public async Task<IEnumerable<Folder>> GetALlFolders()
        {
            return await dbContext.Folders.ToListAsync();
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
