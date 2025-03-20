using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "CategoryMenu";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);

        public CategoryMenuViewComponent(ICategoryService categoryService, IMemoryCache cache)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IViewComponentResult> InvokeAsync(bool showFeaturedOnly = false)
        {
            var categories = await GetCategoriesFromCacheAsync(showFeaturedOnly);
            return View(categories); // No need to specify view name if using Default
        }

        private async Task<IEnumerable<Category>> GetCategoriesFromCacheAsync(bool showFeaturedOnly)
        {
            string cacheKey = showFeaturedOnly ? $"{CacheKey}_Featured" : CacheKey;

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Category> categories))
            {
                int pageNumber = 1;
                int pageSize = showFeaturedOnly ? 10 : 50;
                string sortBy = "name_asc";

                categories = await _categoryService.GetAllCategoriesAsync(pageNumber, pageSize, sortBy);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheExpiration)
                    .SetPriority(CacheItemPriority.High);

                _cache.Set(cacheKey, categories, cacheOptions);
            }

            return categories;
        }
    }
}
