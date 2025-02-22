using AutoMapper;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;

namespace TechXpress_application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.FinalPrice,
                    opt => opt.MapFrom(src => src.DiscountPrice ?? src.Price))
                .ForMember(dest => dest.Rating,
                    opt => opt.MapFrom(src => src.Reviews.Any() ?
                        src.Reviews.Average(r => r.Rating) : 0));
        }
    }
}