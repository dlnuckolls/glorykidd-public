using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlazorAppWeb.Models;

namespace BlazorAppWeb.Data
{
    public class BlazorAppWebQuotesContext : DbContext
    {
        public BlazorAppWebQuotesContext (DbContextOptions<BlazorAppWebQuotesContext> options)
            : base(options)
        {
        }

        public DbSet<BlazorAppWeb.Models.Quotes> Quotes { get; set; } = default!;
    }
}
