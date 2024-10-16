using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AnonymousChatApi.Databases.Models;

[Serializable]
[Table("Chats", Schema = AnonymousChatDbContext.PublicSchema)]
[Index(nameof(Id), IsUnique = true)]
public sealed record Chat
{
    [Key]
    public long Id { get; set; }
    [MaxLength(50)]
    public required string Name { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required bool IsDeleted { get; set; }
    public required List<ChatUser> ChatUsers { get; set; }
    public required List<MessageBase> Messages { get; set; }
}
