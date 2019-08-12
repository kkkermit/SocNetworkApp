using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocNetworkApp.API.Dtos;
using SocNetworkApp.API.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SocNetworkApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userNamager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration config,
                              IMapper mapper,
                              UserManager<User> userNamager,
                              SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _config = config;
            _userNamager = userNamager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            User userToCreate = _mapper.Map<User>(userRegisterDto);

            IdentityResult result = await _userNamager.CreateAsync(userToCreate, userRegisterDto.Password);

            UserDetailedDto userDetailedDto = _mapper.Map<UserDetailedDto>(userToCreate);

            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userDetailedDto);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            User user = await _userNamager.FindByNameAsync(userLoginDto.Username);

            if (user != null)
            {
                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

                if (result.Succeeded)
                {
                    User appUser = await _userNamager.Users.Include(p => p.Photos)
                                                           .FirstOrDefaultAsync(u => u.NormalizedUserName == userLoginDto.Username.ToUpper());

                    UserListDto userListDto = _mapper.Map<UserListDto>(appUser);

                    return Ok(new 
                    {
                        token = await GenerateJwtToken(appUser),
                        user = userListDto
                    });
                }
            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            IList<string> roles = await _userNamager.GetRolesAsync(user);

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}