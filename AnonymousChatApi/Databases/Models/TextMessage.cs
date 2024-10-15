using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnonymousChatApi.Databases.Models;

[Serializable]
[Table("TextMessages", Schema = AnonymousChatDbContext.PublicSchema)]
public class TextMessage
{
    
    [Key] public required long MessageId { get; set; }
    
    public required string Text { get; set; }   
}