using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "CategoryMenuCategories";

        public CategoryMenuViewComponent(ICategoryService categoryService, IMemoryCache cache)
        {
            _categoryService = categoryService;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
             int pageNumber = 1;
            int pageSize = 50;  
            string sortBy = "name_asc"; 

            // Try to get the list of categories from the cache.
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<Category> categories))
            {
                categories = await _categoryService.GetAllCategoriesAsync(pageNumber, pageSize, sortBy);

                // Cache the categories for 15 minutes (adjust expiration as needed)
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                _cache.Set(CacheKey, categories, cacheOptions);
            }

            return View(categories);
        }
    }
}
