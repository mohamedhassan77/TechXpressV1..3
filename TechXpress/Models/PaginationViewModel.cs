using System;
using System.Collections.Generic;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class PaginationViewModel<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int PageCount => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public List<T> Items { get; set; }

    }
}
