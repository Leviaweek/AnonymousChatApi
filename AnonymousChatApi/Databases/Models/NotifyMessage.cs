using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnonymousChatApi.Databases.Models;

[Serializable]
[Table("NotifyMessages", Schema = AnonymousChatDbContext.PublicSchema)]
public class NotifyMessage
{
    [Key] public required int MessageId { get; set; }
    
    public required NotifyType Type { get; set; }
}