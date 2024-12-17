using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationREST.Data
{
    public class WebApplicationRESTContext : DbContext
    {
        public WebApplicationRESTContext (DbContextOptions<WebApplicationRESTContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; } = default!;
    }
}
