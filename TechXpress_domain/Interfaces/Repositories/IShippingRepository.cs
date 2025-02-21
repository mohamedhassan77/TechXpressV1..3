using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface IShippingRepository
    {
        Task<Shipping?> GetShippingByOrderIdAsync(int orderId);
        Task<IEnumerable<Shipping>> GetAllShipmentsAsync();
        Task AddShippingAsync(Shipping shipping);
        Task UpdateShippingAsync(Shipping shipping);
        Task DeleteShippingAsync(int id);
        Task SaveChangesAsync();
    }
}
