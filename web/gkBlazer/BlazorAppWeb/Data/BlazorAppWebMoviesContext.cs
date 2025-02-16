using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlazorAppWeb.Models;

namespace BlazorAppWebMovies.Data
{
    public class BlazorAppWebMoviesContext : DbContext
    {
        public BlazorAppWebMoviesContext (DbContextOptions<BlazorAppWebMoviesContext> options)
            : base(options)
        {
        }

        public DbSet<BlazorAppWeb.Models.Movie> Movie { get; set; } = default!;
    }
}
