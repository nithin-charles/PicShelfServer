using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PicShelfServer.Models.Domain;
using PicShelfServer.Models.DTO;
using PicShelfServer.Services;

namespace PicShelfServer.Controllers
{
    public class ImageController : ControllerBase
    {

        private readonly IImageService _imageService;
        private readonly string _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

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
        [Route("GetPicUrl")]
        public async Task<string> GetPicById(Guid id)
        {
            return await _imageService.GetImageUrl(id);
        }

        [HttpGet]
        [Route("GetPicUrl/{folderName}")]
        public async Task<IReadOnlyCollection<Image>> GetPicByFolder(string folderName)
        {
            return await _imageService.GetPicByFolder(folderName);
        }

        [HttpGet]
        [Route("GetPic/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            try
            {
                // Construct the file path
                var filePath = Path.Combine(_imageFolderPath, imageName);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                    return NotFound("Image not found.");

                // Determine the file's content type
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                    contentType = "application/octet-stream";

                // Read the file and return as a response
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete]
        [Route("DeletePic/{id}")]
        public ActionResult DeleteImage(Guid id)
        {
            try
            {
                _imageService.DeletePic(id);
                return StatusCode(204);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("MoveToFolder")]
        public async Task<Image> MoveTFolder(Guid id, string folderName)
        {
            return await _imageService.MoveToFolder(id, folderName);
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

        [HttpGet]
        [Route("GetAllFolders")]
        public async Task<IEnumerable<Folder>> GetAllFolders()
        {
            return await _imageService.GetALlFolders();
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
