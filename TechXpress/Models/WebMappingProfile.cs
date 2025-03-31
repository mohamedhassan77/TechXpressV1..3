using AutoMapper;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress.Models;
using System;

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
            CreateMap<CategoryResponseDto, CategoryViewModel>();

            // -----------------------------
            // Review Mappings
            // -----------------------------
            CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty));
            CreateMap<ReviewDashboardDto, ReviewViewModel>();
            // Consolidate duplicate mapping if needed:
            CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser.UserName ?? "N/A"))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name ?? "N/A"))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty));

            // -----------------------------
            // User Profile Mappings
            // -----------------------------
            CreateMap<UserProfile, UserProfileViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ApplicationUser.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ApplicationUser.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicationUser.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                    $"{src.ApplicationUser.FirstName} {src.ApplicationUser.LastName}".Trim()))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src =>
                    src.ProfileImageData != null && src.ProfileImageData.Length > 0
                        ? "data:image/jpeg;base64," + Convert.ToBase64String(src.ProfileImageData)
                        : "/images/default-avatar.png"));

            // -----------------------------
            // Map from Review to ReviewDashboardDto (if needed)
            // -----------------------------
            CreateMap<Review, ReviewDashboardDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product == null ? "N/A" : src.Product.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser == null ? "N/A" : src.ApplicationUser.FirstName))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // -----------------------------
            // Order Mappings
            // -----------------------------
            // Map from OrderDto to OrderDetailsViewModel.
            CreateMap<OrderDto, OrderDetailsViewModel>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.orderId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.Status));
         }
    }
}
