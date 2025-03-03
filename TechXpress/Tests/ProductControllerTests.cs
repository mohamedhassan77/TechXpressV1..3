using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TechXpress.Controllers;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TechXpress.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly Mock<IWishlistService> _wishlistServiceMock;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _wishlistServiceMock = new Mock<IWishlistService>();
            _reviewServiceMock = new Mock<IReviewService>();
            _cartServiceMock = new Mock<ICartService>();
            _loggerMock = new Mock<ILogger<ProductController>>();

            _controller = new ProductController(
                _productServiceMock.Object,
                _categoryServiceMock.Object,
                _wishlistServiceMock.Object,
                _cartServiceMock.Object,
                _reviewServiceMock.Object
            );

            // Set up a dummy authenticated user.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId"),
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task Index_ReturnsView_WithFilteredProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10M },
                new Product { Id = 2, Name = "Product 2", Price = 20M }
            };

            _productServiceMock
                .Setup(s => s.GetFilteredProductsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((products, products.Count));

            _categoryServiceMock
                .Setup(s => s.GetAllCategoriesAsync(1, 10, "name_asc"))
                .ReturnsAsync(new List<Category>());

            // Act
            var result = await _controller.Index("TestCategory", "Test", "name_asc", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UnifiedProductViewModel>(viewResult.Model);
            Assert.Equal(products.Count, model.PaginationInfo.TotalItems);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            _productServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WithProductDetails()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 15M,
                Description = "Test Description",
                Reviews = new List<Review>()
            };

            _productServiceMock.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(product);
            _productServiceMock.Setup(s => s.GetRelatedProductsAsync(1))
                .ReturnsAsync(new List<Product>());
            _wishlistServiceMock.Setup(s => s.GetWishlistAsync(It.IsAny<string>()))
                .ReturnsAsync((Wishlist)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UnifiedProductViewModel>(viewResult.Model);
            Assert.Equal(product.Id, model.ProductDetails.Id);
            Assert.Equal("Test Product", model.ProductDetails.Name);
        }

    }
    
}
