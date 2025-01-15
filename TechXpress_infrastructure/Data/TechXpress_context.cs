using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;

namespace TechXpress_infrastructure.Data
{
    public class TechXpress_context:DbContext
    {
        public TechXpress_context(DbContextOptions<TechXpress_context> options):base(options) 
        {
        }
        
         public DbSet<product> products {  get; set; }
    }
}
