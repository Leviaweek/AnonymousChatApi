using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnonymousChatApi.Databases.Models;

[Serializable]
[Table("ChatUsers", Schema = AnonymousChatDbContext.PublicSchema)]
[EntityTypeConfiguration<ChatUserConfigure, ChatUser>]
public sealed record ChatUser
{
    public required long UserId { get; set; }
    
    [MaxLength(50)]
    public required string UserName { get; set; }
    public required long ChatId { get; set; }
    public required long? LastReadMessageId { get; set; }
    public required Chat Chat { get; set; }
    public required User User { get; set; }
}

file sealed class ChatUserConfigure: IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.HasKey(cu => new { cu.UserId, cu.ChatId });

        builder.HasOne(x => x.Chat)
            .WithMany(x => x.ChatUsers)
            .HasForeignKey(cu => cu.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(cu => cu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<MessageBase>()
            .WithMany()
            .HasForeignKey(m => m.LastReadMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}