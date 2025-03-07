namespace TechXpress_domain.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; }
        public string Specifications { get; set; }
        public decimal Rating { get; set; }
    }
}
