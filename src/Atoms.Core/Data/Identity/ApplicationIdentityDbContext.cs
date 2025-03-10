using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atoms.Core.Data.Identity;

public class ApplicationIdentityDbContext(
    DbContextOptions<ApplicationIdentityDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
}