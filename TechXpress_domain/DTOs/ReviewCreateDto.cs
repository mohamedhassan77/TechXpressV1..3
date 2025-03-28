using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechXpress_domain.DTOs
{

    // DTO classes (if not defined elsewhere)
    public class ReviewCreateDto
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
    

}
