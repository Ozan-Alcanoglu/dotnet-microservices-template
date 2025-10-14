using AutoMapper;
using UserService.DTO;
using UserService.Model;

namespace UserService.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Model'den UserDto'ya ve tersi yönde eşleştirme (mapping)
            CreateMap<User, UserDto>().ReverseMap();

            // UserCreateDto'dan User Model'e eşleştirme
            CreateMap<UserCreateDto, User>();
        }
    }
}