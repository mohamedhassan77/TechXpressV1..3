using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly TechXpress_context _context;

        public ShippingRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<Shipping?> GetShippingByOrderIdAsync(int orderId)
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.OrderId == orderId);
        }

        public async Task<IEnumerable<Shipping>> GetAllShipmentsAsync()
        {
            return await _context.Shippings
                .Include(s => s.Order)
                .ToListAsync();
        }

        public async Task AddShippingAsync(Shipping shipping)
        {
            await _context.Shippings.AddAsync(shipping);
        }

        public async Task UpdateShippingAsync(Shipping shipping)
        {
             _context.Shippings.Update(shipping);
        }

        public async Task DeleteShippingAsync(int id)
        {
            var shipping = await _context.Shippings.FindAsync(id);
            if (shipping != null)
            {
                _context.Shippings.Remove(shipping);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
