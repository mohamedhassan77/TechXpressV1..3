using AutoMapper;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;

namespace TechXpress_application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // -----------------------------
            // Category Mappings
            // -----------------------------
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // -----------------------------
            // Product Mappings
            // -----------------------------
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.DiscountPrice ?? src.Price))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Reviews != null && src.Reviews.Any()
                                                                        ? src.Reviews.Average(r => r.Rating)
                                                                        : 0));

            CreateMap<ProductFilterDto, ProductFilterDto>();



            CreateMap<ApplicationUser, AuthResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.Success, opt => opt.Ignore())
                .ForMember(dest => dest.Token, opt => opt.Ignore());
        }
    }
}
