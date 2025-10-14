using AutoMapper;
using AuthService.DTO;
using AuthService.Model;

namespace AuthService.Mapper
{
    // Bu sınıf, Entity'ler ve DTO'lar arasındaki dönüşümü tanımlar.
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LoginDto, AuthUser>(); 
        }
    }
}