using PicShelfServer.Models.Domain;

namespace PicShelfServer.Services
{
    public interface IImageService
    {
        Task<Image> Upload(Image image);
        Task<IReadOnlyCollection<Image>> GetAllPics();
        Task<string> GetImageUrl(Guid imageId);
    }
}
