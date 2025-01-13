using Atoms.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Atoms.Infrastructure.Data.Identity;

public class ApplicationIdentityDbContext(
    DbContextOptions<ApplicationIdentityDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
}