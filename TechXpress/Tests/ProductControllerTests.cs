using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TechXpress.Controllers;
using TechXpress_domain.Entities;
using TechXpress_application.Interfaces;
using Xunit;

namespace TechXpress.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IWishlistRepository> _wishlistRepositoryMock;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            // Create mocks for the repository interfaces
            _productRepositoryMock = new Mock<IProductRepository>();
            _wishlistRepositoryMock = new Mock<IWishlistRepository>();
            _cartRepositoryMock = new Mock<ICartRepository>();

            // For UserManager, we need a minimal IUserStore<ApplicationUser> implementation
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _loggerMock = new Mock<ILogger<ProductController>>();

            // Instantiate the ProductController with the mocked dependencies
            _controller = new ProductController(
                _loggerMock.Object,
                _productRepositoryMock.Object,
                _wishlistRepositoryMock.Object,
                _cartRepositoryMock.Object,
                _userManagerMock.Object
            );

            // Set up a dummy HttpContext with an authenticated user for controller actions
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId"),
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var identity = new ClaimsIdentity(userClaims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task Index_ReturnsView_WithFeaturedProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10M },
                new Product { Id = 2, Name = "Product 2", Price = 20M }
            };
            _productRepositoryMock.Setup(repo => repo.GetFeaturedProductsAsync())
                                  .ReturnsAsync(products);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange: Setup repository to return null for a given product id
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsView_WithProductDetailsViewModel()
        {
            // Arrange: Create a sample product and set up repository
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 15M,
                Description = "Test Description"
            };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                                  .ReturnsAsync(product);

            // Act: Call the Details action
            var result = await _controller.Details(1);

            // Assert: Verify that we get a view result with a ProductDetailsViewModel as its model
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<ProductDetailsViewModel>(viewResult.Model);
            Assert.Equal(product.Id, viewModel.Product.Id);
            Assert.Equal("Test Product", viewModel.Product.Name);
        }

        [Fact]
        public async Task Create_Post_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 15M
            };

            // Setup the repository to complete successfully
            _productRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Ensure ModelState is valid (by not adding any errors)
            // Act
            var result = await _controller.Create(product);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _productRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Product>(
                p => p.Name == product.Name &&
                     p.Description == product.Description &&
                     p.Price == product.Price
            )), Times.Once);
        }
    }
}
