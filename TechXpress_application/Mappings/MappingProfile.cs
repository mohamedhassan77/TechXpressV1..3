using AutoMapper;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using System;
using System.Linq;

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

            // -----------------------------
            // Review Mappings
            // -----------------------------
            CreateMap<Review, ReviewDashboardDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product == null ? "N/A" : src.Product.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser == null ? "N/A" : src.ApplicationUser.FirstName))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // -----------------------------
            // Authentication Mapping
            // -----------------------------
            CreateMap<ApplicationUser, AuthResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Message, opt => opt.Ignore())
                .ForMember(dest => dest.Success, opt => opt.Ignore())
                .ForMember(dest => dest.Token, opt => opt.Ignore());

            // -----------------------------
            // User Profile Mappings
            // -----------------------------
            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.UserName : "N/A"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.Email : ""))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsBlocked));
            CreateMap<UserProfileDto, UserProfile>()
                .ForMember(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.UserId));

            // -----------------------------
            // Order Mappings
            // -----------------------------
                 CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.orderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.OrderItems != null ? src.OrderItems.Count : 0))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));
        }
    }
}
