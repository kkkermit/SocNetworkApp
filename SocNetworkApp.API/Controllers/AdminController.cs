using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocNetworkApp.API.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocNetworkApp.API.Dtos;
using Microsoft.AspNetCore.Identity;
using SocNetworkApp.API.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SocNetworkApp.API.Helpers;
using CloudinaryDotNet;
using System;
using CloudinaryDotNet.Actions;

namespace SocNetworkApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {

            _context = context;
            _userManager = userManager;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account()
            {
                Cloud = _cloudinaryConfig.Value.CloudName,
                ApiKey = _cloudinaryConfig.Value.ApiKey,
                ApiSecret = _cloudinaryConfig.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(acc);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await (from user in _context.Users
                               orderby user.UserName
                               select new
                               {
                                   Id = user.Id,
                                   Username = user.UserName,
                                   Roles = (from userRole in user.UserRoles
                                            join role in _context.Roles
                                            on userRole.RoleId equals role.Id
                                            select role.Name).ToList()
                               }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            User user = await _userManager.FindByNameAsync(userName);

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            string[] selectedRoles = roleEditDto.RoleNames;

            selectedRoles = selectedRoles ?? new string[] { };

            IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photos
            .Include(u => u.User)
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new
            {
                Id = u.Id,
                Username = u.User.UserName,
                Url = u.Url,
                IsApproved = u.IsApproved
            }).ToListAsync();

            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(Guid photoId)
        {
            Photo photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo != null)
            {
                photo.IsApproved = true;

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(Guid photoId)
        {
            Photo photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo != null)
            {
                if (photo.IsMain)
                {
                    return BadRequest("You cannot reject the main photo");
                }

                if (string.IsNullOrEmpty(photo.PublicId))
                {
                    _context.Photos.Remove(photo);
                }
                else
                {
                    DeletionParams deletionParams = new DeletionParams(photo.PublicId);

                    DeletionResult deletionResult = _cloudinary.Destroy(deletionParams);

                    if (deletionResult.Result == "ok")
                    {
                        _context.Photos.Remove(photo);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}