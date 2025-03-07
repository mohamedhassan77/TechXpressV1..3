using AutoMapper;
using TechXpress_domain.DTOs;
using TechXpress.Models;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            CreateMap<ProductResponseDto, ProductViewModel>()
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.DiscountPrice ?? src.Price));
            CreateMap<ProductCreateDto, ProductViewModel>();
            CreateMap<ProductViewModel, ProductUpdateDto>();
            CreateMap<CategoryCreateDto, CategoryViewModel>();
            CreateMap<CategoryViewModel, CategoryViewModel>();
            CreateMap<UserProfile, UserProfileViewModel>()

                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.ApplicationUser.FirstName} {src.ApplicationUser.LastName}"));
        }
    }
}
