using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Discord_Clone.Server.Models;

namespace Discord_Clone.Server.Data
{
    public class DiscordCloneDbContext : IdentityDbContext<User>
    {
        DbSet<UserFreinds> UserFreinds { get; set; }
        DbSet<UserFreindRequests> UserFreindRequests { get; set; }
        DbSet<Chat> Chats { get; set; }
        DbSet<Message> Messages { get; set; }

        public DiscordCloneDbContext(DbContextOptions<DiscordCloneDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>(m =>
            {
                m.HasKey(m => m.MessageId);

                m.HasOne(m => m.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ChatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Chat>(c =>
            {
                c.HasKey(c => c.ChatId);
            });

            builder.Entity<UserFreinds>(uf =>
            {
                uf.HasKey(uf => uf.UserFriendsId);

                uf.HasOne(uf => uf.Sender)
                    .WithMany(u => u.UserFreinds)
                    .HasForeignKey(uf => uf.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                uf.HasOne(uf => uf.Receiver)
                    .WithMany(u => u.UserFreinds)
                    .HasForeignKey(uf => uf.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                uf.HasOne(uf => uf.Chat)
                    .WithOne(c => c.UserFreinds)
                    .HasForeignKey<UserFreinds>(uf => uf.ChatID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UserFreindRequests>(ufr =>
            {
                ufr.HasKey(ufr => ufr.FriendRequestId);

                ufr.HasOne(ufr => ufr.Sender)
                    .WithMany(u => u.UserFreindRequests)
                    .HasForeignKey(ufr => ufr.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                ufr.HasOne(ufr => ufr.Reciever)
                    .WithMany(u => u.UserFreindRequests)
                    .HasForeignKey(ufr => ufr.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
