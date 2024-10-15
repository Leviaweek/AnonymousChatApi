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
    public required long ChatId { get; set; }

    public Chat? Chat { get; set; }
}

file sealed class ChatUserConfigure: IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> builder)
    {
        builder.HasKey(cu => new { cu.UserId, cu.ChatId });

        builder.HasOne(x => x.Chat)
            .WithMany(c => c.ChatUsers)
            .HasForeignKey(cu => cu.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany(c => c.ChatUsers)
            .HasForeignKey(cu => cu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}