using AnonymousChatApi.Databases.Models;
using Microsoft.EntityFrameworkCore;

namespace AnonymousChatApi.Databases;

[Serializable]
public class AnonymousChatDbContext(DbContextOptions<AnonymousChatDbContext> options): DbContext(options)
{
    public const string PublicSchema = "public";
    
    public required DbSet<Chat> Chats { get; set; }
    public required DbSet<User> Users { get; set; }
    
    public required DbSet<ChatUser> ChatUsers { get; set; }
    public required DbSet<MessageBase> MessageBases { get; set; }
    
    public required DbSet<NotifyMessage> NotifyMessages { get; set; }
    public required DbSet<TextMessage> TextMessages { get; set; }
}