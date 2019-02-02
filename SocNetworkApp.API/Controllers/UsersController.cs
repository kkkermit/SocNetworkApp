using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocNetworkApp.API.Data;
using SocNetworkApp.API.Dtos;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IDataRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IDataRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;   
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            IEnumerable<User> users = await _repository.GetUsers();
            IEnumerable<UserListDto> userListDtos = _mapper.Map<IEnumerable<UserListDto>>(users);

            return Ok(userListDtos);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            User user = await _repository.GetUser(id);
            UserDetailedDto userDetailedDto = _mapper.Map<UserDetailedDto>(user);

            return Ok(userDetailedDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto userUpdateDto)
        {
            if (id != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            User user = await _repository.GetUser(id);
            _mapper.Map(userUpdateDto, user);

            if (await _repository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Updating user {id} failed on save");
        }
    }
}