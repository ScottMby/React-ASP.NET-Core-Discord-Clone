using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Discord_Clone.Server.Models;

namespace Discord_Clone.Server.Data
{
    public class DiscordCloneDbContext(DbContextOptions<DiscordCloneDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<UserFriends> UserFriends { get; set; }
        public DbSet<UserFriendRequests> UserFriendRequests { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(u =>
            {
                u.HasGeneratedTsVectorColumn(
                    u => u.UserSearchVector,
                    "english",
                    u => new { u.DisplayName })
                    .HasIndex(u => u.UserSearchVector)
                    .HasMethod("GIN");
            });

            builder.Entity<Message>(m =>
            {
                m.HasKey(m => m.MessageId);
                m.Property(e => e.MessageId).ValueGeneratedOnAdd();

                m.HasOne(m => m.Chat)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ChatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Chat>(c =>
            {
                c.HasKey(c => c.ChatId);
                c.Property(c => c.ChatId).ValueGeneratedOnAdd();
            });

            builder.Entity<UserFriends>(uf =>
            {
                uf.HasKey(uf => uf.UserFriendsId);
                uf.Property(e => e.UserFriendsId).ValueGeneratedOnAdd();

                uf.HasOne(uf => uf.Sender)
                    .WithMany(u => u.SentUserFriends)
                    .HasForeignKey(uf => uf.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                uf.HasOne(uf => uf.Receiver)
                    .WithMany(u => u.ReceivedUserFriends)
                    .HasForeignKey(uf => uf.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                uf.HasOne(uf => uf.Chat)
                    .WithOne(c => c.UserFriends)
                    .HasForeignKey<UserFriends>(uf => uf.ChatID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<UserFriendRequests>(ufr =>
            {
                ufr.HasKey(ufr => ufr.FriendRequestId);
                ufr.Property(e => e.FriendRequestId).ValueGeneratedOnAdd();

                ufr.HasOne(ufr => ufr.Sender)
                    .WithMany(u => u.SentUserFriendRequests)
                    .HasForeignKey(ufr => ufr.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                ufr.HasOne(ufr => ufr.Receiver)
                    .WithMany(u => u.ReceivedUserFriendRequests)
                    .HasForeignKey(ufr => ufr.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
