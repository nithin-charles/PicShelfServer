using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicShelfServer.Models.Domain;
using PicShelfServer.Models.DTO;
using PicShelfServer.Services;

namespace PicShelfServer.Controllers
{
    public class ImageController : ControllerBase
    {

        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            this._imageService = imageService;
        }

        [HttpGet]
        [Route("GetAllPics")]
        public async Task<IReadOnlyCollection<Image>> GetAllPics()
        {
            return await _imageService.GetAllPics();
        }
        
        [HttpGet]
        [Route("GetPic")]
        public async Task<string> GetPicById(Guid id)
        {
            return await _imageService.GetImageUrl(id);
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if (ModelState.IsValid)
            {
                // convert DTO to Domain model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription,
                    FolderName = request.FolderName != null ? request.FolderName : "Others"
                };


                // User repository to upload image
                await _imageService.Upload(imageDomainModel);

                return Ok(imageDomainModel);

            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("AddFolder")]
        public Task<Folder> AddFolder(string folderName)
        {
            return _imageService.AddFolder(folderName);
        }

        [HttpPost]
        [Route("DeleteFolder")]
        public Task<Folder> DeleteFolder(string folderName)
        {
            return _imageService.RemoveFolder(folderName);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file.");
            }
        }
    }
}
