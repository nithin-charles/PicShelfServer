using System.ComponentModel.DataAnnotations;

namespace PicShelfServer.Models.DTO
{
    public class LoginRequestDto
    {
        [DataType(DataType.EmailAddress)]
        public required string Username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
