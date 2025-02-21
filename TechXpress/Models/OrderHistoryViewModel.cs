using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class OrderHistoryViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Orders")]
        public List<Order> Orders { get; set; } = new List<Order>();

        [Display(Name = "Total Orders")]
        public int TotalOrders => Orders?.Count ?? 0;

        [Display(Name = "Date Range")]
        public string DateRange { get; set; }

        [Display(Name = "Filter Status")]
        public string FilterStatus { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "DateDesc";

        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int TotalPages { get; set; }
        public bool HasMoreOrders { get; set; }

    }
}
