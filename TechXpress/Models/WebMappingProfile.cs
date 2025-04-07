using AutoMapper;
using System;
using System.Linq;
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
            CreateMap<CategoryResponseDto, CategoryViewModel>();

            // -----------------------------
            // Review Mappings
            // -----------------------------
            // Mapping for displaying reviews in the UI.
            CreateMap<Review, ReviewViewModel>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser != null ? src.ApplicationUser.UserName : "N/A"))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "N/A"));
            CreateMap<ReviewDashboardDto, ReviewViewModel>();

             CreateMap<Review, ReviewDashboardDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product == null ? "N/A" : src.Product.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser == null ? "N/A" : src.ApplicationUser.FirstName))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

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
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src =>
                    src.ProfileImageData != null && src.ProfileImageData.Length > 0
                        ? "data:image/jpeg;base64," + Convert.ToBase64String(src.ProfileImageData)
                        : "/images/default-avatar.png"));

            // When mapping back from the view model to the domain, we want to update the ApplicationUser fields.
            CreateMap<UserProfileViewModel, UserProfile>()
                .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => new ApplicationUser
                {
                    Id = src.UserId,
                    FirstName = src.FirstName,
                    LastName = src.LastName,
                    Email = src.Email,
                    PhoneNumber = src.PhoneNumber,
                    // Depending on your design, you might want to map Addresses and Orders or handle them separately.
                    Addresses = src.Addresses,
                    DateOfBirth = src.DateOfBirth
                    // Note: Do not map ProfileImageData here if you’re handling file uploads separately.
                }));

            // Map from UserProfile to UserProfileDto and vice versa.
            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ApplicationUser.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicationUser.Email))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsBlocked));
            CreateMap<UserProfileDto, UserProfile>()
                .ForMember(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.UserId));

            // -----------------------------
            // Order Mappings
            // -----------------------------
            // Map from Order entity to OrderDto.
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.OrderItems != null ? src.OrderItems.Count : 0));
            // Map from OrderDto to OrderDetailsViewModel (for UI display)
            CreateMap<OrderDto, OrderDetailsViewModel>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.orderId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.Status));

            // Map from OrderUpdateDto to Order entity.
            CreateMap<OrderUpdateDto, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NewStatus));
        }
    }
}
