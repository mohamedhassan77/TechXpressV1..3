using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IShippingService
    {
        Task<Shipping?> GetShippingByOrderIdAsync(int orderId);
        Task<IEnumerable<Shipping>> GetAllShipmentsAsync();
        Task<string> CreateShippingAsync(int orderId, string carrier, DateTime estimatedDelivery);
        Task<string> UpdateShippingStatusAsync(int orderId, string status, string? trackingNumber = null);
    }
}
