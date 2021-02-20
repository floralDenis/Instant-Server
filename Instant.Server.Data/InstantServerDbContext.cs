using System.Data.Entity;
using Instant.Server.Data.Entities;

namespace Instant.Server.Data
{
    public class InstantServerDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ChatPermission> ChatPermissions { get; set; }
        public virtual DbSet<ChatDeletion> ChatDeletions { get; set; }
        public virtual DbSet<ChatMessageDeletion> ChatMessageDeletions { get; set; }
        
        public InstantServerDbContext() : base("default")
        { }
    }
}