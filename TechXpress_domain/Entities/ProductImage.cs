using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress_domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        [Url(ErrorMessage = "Invalid image URL.")]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }

}
