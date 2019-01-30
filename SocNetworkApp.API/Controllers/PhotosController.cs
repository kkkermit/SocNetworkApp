using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocNetworkApp.API.Data;
using SocNetworkApp.API.Dtos;
using SocNetworkApp.API.Helpers;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    [Authorize]
    public class PhotosController : ControllerBase
    {
        private readonly IDataRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDataRepository repository, 
                                IMapper mapper, 
                                IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account()
            {
                Cloud = _cloudinaryConfig.Value.CloudName,
                ApiKey = _cloudinaryConfig.Value.ApiKey,
                ApiSecret = _cloudinaryConfig.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(Guid id)
        {
            Photo photo = await _repository.GetPhoto(id);

            PhotoReturnDto photoReturnDto = _mapper.Map<PhotoReturnDto>(photo);

            return Ok(photoReturnDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(Guid userId, [FromForm]PhotoCreationDto photoCreationDto)
        {
            if (userId != Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
            {
                return Unauthorized();
            }

            User user = await _repository.GetUser(userId);

            IFormFile file = photoCreationDto.File;

            ImageUploadResult uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using(Stream stream = file.OpenReadStream())
                {
                    ImageUploadParams uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500)
                                                             .Height(500)
                                                             .Crop("fill")
                                                             .Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoCreationDto.Url = uploadResult.Uri.ToString();
            photoCreationDto.PublicId = uploadResult.PublicId;

            Photo photo = _mapper.Map<Photo>(photoCreationDto);

            if (!user.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _repository.SaveAll())
            {
                PhotoReturnDto photoReutrnDto = _mapper.Map<PhotoReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoReutrnDto);
            }

            return BadRequest("Could not add the photo");
        }
    }
}