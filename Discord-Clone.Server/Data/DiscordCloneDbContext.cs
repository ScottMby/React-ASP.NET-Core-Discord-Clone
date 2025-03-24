using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Discord_Clone.Server.Data
{
    public class DiscordCloneDbContext : IdentityDbContext<IdentityUser>
    {
        public DiscordCloneDbContext(DbContextOptions<DiscordCloneDbContext> options) : base(options)
        {
            
        }
    }
}
