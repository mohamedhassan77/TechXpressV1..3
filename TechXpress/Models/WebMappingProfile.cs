using AutoMapper;
using System;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress.Models;

namespace TechXpress.Models
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            // -----------------------------
            // Product Mappings
            // -----------------------------
            CreateMap<ProductResponseDto, ProductViewModel>()
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.DiscountPrice ?? src.Price));

            CreateMap<ProductCreateDto, ProductViewModel>();
            CreateMap<ProductViewModel, ProductUpdateDto>();
            CreateMap<Product, ProductViewModel>();
          
            // -----------------------------
            // Category Mappings
            // -----------------------------
            CreateMap<CategoryCreateDto, CategoryViewModel>();
            CreateMap<Category, CategoryViewModel>()
              .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

            CreateMap<Category, CategoryViewModel>();
            CreateMap<Review, ReviewViewModel>()
    .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty));

            // -----------------------------
            // User Profile Mappings
            // -----------------------------
            CreateMap<UserProfile, UserProfileViewModel>()
     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
         ((src.ApplicationUser != null ? src.ApplicationUser.FirstName : string.Empty) + " " +
          (src.ApplicationUser != null ? src.ApplicationUser.LastName : string.Empty)).Trim()))
     .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src =>
         (src.ProfileImageData != null && src.ProfileImageData.Length > 0)
             ? "data:image/jpeg;base64," + Convert.ToBase64String(src.ProfileImageData)
             : "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png"));



        }
    }
}
