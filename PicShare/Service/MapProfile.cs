using AutoMapper;
using Common.Dto;
using Repository.Entity;

namespace Service
{
    public class MapProfile : Profile
    {

        public MapProfile()
        {
            CreateMap<Photo, PhotoResponseDto>();
            CreateMap<PhotoUploadDto, Photo>();
            CreateMap<UserSignUpDto, User>();
            CreateMap<User, UserResponseDto>();
        }
    }
}

