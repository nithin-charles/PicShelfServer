using System.ComponentModel.DataAnnotations;

namespace PicShelfServer.Models.DTO
{
    public class ImageUploadRequestDto
    {
        public required IFormFile File { get; set; }

        public required string FileName { get; set; }

        public string? FileDescription { get; set; }
    }
}
