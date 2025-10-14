using AutoMapper;
using ProductService.DTO;
using ProductService.Model;

namespace ProductService.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductCreateDto, Product>();
        }
    }
}