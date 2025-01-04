using PicShelfServer.Models.Domain;

namespace PicShelfServer.Services
{
    public interface IImageService
    {
        Task<Image> Upload(Image image);
        Task<IReadOnlyCollection<Image>> GetAllPics();
        Task<string> GetImageUrl(Guid imageId);
        void DeletePic(Guid id);
        Task<IEnumerable<Folder>> GetALlFolders();
        Task<IReadOnlyCollection<Image>> GetPicByFolder(string folderName);
        Task<Folder> AddFolder(string folderName);
        Task<Folder?> RemoveFolder(string folderName);
        Task<Image> MoveToFolder(Guid imageId, string folderName);
    }
}
