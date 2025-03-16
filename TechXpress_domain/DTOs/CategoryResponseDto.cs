using System;
using System.Collections.Generic;
using TechXpress_domain.DTOs;

namespace TechXpress_domain.DTOs
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
    }
}
