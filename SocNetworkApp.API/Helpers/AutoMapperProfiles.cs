using System.Linq;
using AutoMapper;
using SocNetworkApp.API.Dtos;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => { 
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });;
            CreateMap<User, UserDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => { 
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotoDetailedDto>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Photo, PhotoReturnDto>();
            CreateMap<PhotoCreationDto, Photo>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<MessageCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageReturnDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}