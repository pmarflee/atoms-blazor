using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atoms.Core.Data.Identity;

public class ApplicationIdentityDbContext(
    DbContextOptions<ApplicationIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public ValueTask<ApplicationUser> GetUserById(UserId userId) =>
        FindAsync<ApplicationUser>(userId.Id)!;
}