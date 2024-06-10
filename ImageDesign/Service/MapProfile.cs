using AutoMapper;
using Common.Dto;
using Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MapProfile : Profile
    {

        public MapProfile()
        {
            CreateMap<Image, ImageDto>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<ImageDto, Image>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<User, UserDto>().ForMember(dest => dest.ProfileImage, opt => opt.Ignore());
            CreateMap<UserDto, User>();
            CreateMap<Task<Image>, Task<ImageDto>>().ReverseMap();
            CreateMap<Task<User>, Task<UserDto>>().ReverseMap();
            CreateMap<Task<List<Image>>, Task<List<ImageDto>>>().ReverseMap();
            CreateMap<Task<List<User>>, Task<List<UserDto>>>().ReverseMap();


        }
    }
}
