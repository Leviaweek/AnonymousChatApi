using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnonymousChatApi.Databases.Models;

[Serializable]
[Table("MessageBases", Schema = AnonymousChatDbContext.PublicSchema)]
[EntityTypeConfiguration<ChatMessagesConfigure, MessageBase>]
public sealed class MessageBase
{
    public required long UserId { get; set; }
    public required long ChatId { get; set; }
    public long Id { get; set; }
    public required DateTimeOffset TimeStamp { get; set; }

    public TextMessage? TextMessage { get; set; }
    public NotifyMessage? NotifyMessage { get; set; }

}

file sealed class ChatMessagesConfigure : IEntityTypeConfiguration<MessageBase>
{
    public void Configure(EntityTypeBuilder<MessageBase> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne<Chat>()
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.TextMessage)
            .WithOne()
            .HasForeignKey<TextMessage>(t => t.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.NotifyMessage)
            .WithOne()
            .HasForeignKey<NotifyMessage>(n => n.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}