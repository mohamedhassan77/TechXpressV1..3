using AutoMapper;
using TechXpress_domain.Entities;
using TechXpress.Models;
using System;
using TechXpress_domain.DTOs;

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
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.ApplicationUser.FirstName} {src.ApplicationUser.LastName}"))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src =>
                    src.ProfileImageData != null && src.ProfileImageData.Length > 0
                        ? "data:image/jpeg;base64," + Convert.ToBase64String(src.ProfileImageData)
                        : "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png"));
        }
    }
}
