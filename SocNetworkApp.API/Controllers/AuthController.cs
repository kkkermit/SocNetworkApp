using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocNetworkApp.API.Data;
using SocNetworkApp.API.Dtos;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        public AuthController(IAuthRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userRegister.Username = userRegister.Username.ToLower();

            if(await _repository.UserExists(userRegister.Username))
            {
                return BadRequest("Username already exists");
            }

            User userToCreate = new User
            {
                Username = userRegister.Username
            };

            User createdUser = await _repository.Register(userToCreate, userRegister.Password);

            return StatusCode(201);
        }
    }
}