using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.DTOs
{
    public class ProductUpdateDto : ProductCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}