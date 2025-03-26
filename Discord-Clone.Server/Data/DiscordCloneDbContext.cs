using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Discord_Clone.Server.Models;

namespace Discord_Clone.Server.Data
{
    public class DiscordCloneDbContext : IdentityDbContext<User>
    {

        public DiscordCloneDbContext(DbContextOptions<DiscordCloneDbContext> options) : base(options)
        {
            
        }
    }
}
