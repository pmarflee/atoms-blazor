using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace Atoms.Infrastructure.Data.DataProtection;

public class DataProtectionKeyContext(DbContextOptions<DataProtectionKeyContext> options) 
    : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
}