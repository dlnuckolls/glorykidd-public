using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace gkquotes.Data {
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): IdentityDbContext<ApplicationUser>(options) {
  }
}
