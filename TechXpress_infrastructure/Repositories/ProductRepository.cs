using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;
using TechXpress_infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace TechXpress_infrastructure.Repositories
{
    public class ProductRepository
    {
        private readonly TechXpress_context _context;

        public ProductRepository(TechXpress_context context)
        {
            _context = context;
        }

        // Get all products
        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }
        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsFeatured)
                .ToListAsync();
        }
        // Get a product by its ID
        public Product GetById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        // Add a new product
        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Update an existing product
        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        // Delete a product
        public void Delete(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}
