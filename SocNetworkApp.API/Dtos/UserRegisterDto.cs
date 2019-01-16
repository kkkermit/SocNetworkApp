using System.ComponentModel.DataAnnotations;

namespace SocNetworkApp.API.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password should contain at least 6 characters.")]
        public string Password { get; set; }
    }
}