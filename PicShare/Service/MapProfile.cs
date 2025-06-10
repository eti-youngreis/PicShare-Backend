using AutoMapper;
using Common.Dto;
using Repository.Entity;

namespace Service
{
    public class MapProfile : Profile
    {

        public MapProfile()
        {
            CreateMap<Image, ImageDto>();
            CreateMap<ImageDto, Image>();
            CreateMap<UserSignupDto, User>();
            CreateMap<User, UserResponseDto>();
        }
    }
}

