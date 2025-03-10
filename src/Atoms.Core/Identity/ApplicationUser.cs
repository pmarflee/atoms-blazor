using Microsoft.AspNetCore.Identity;

namespace Atoms.Core.Identity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = default!;
}
