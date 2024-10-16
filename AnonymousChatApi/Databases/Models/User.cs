using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AnonymousChatApi.Databases.Models;

[Serializable]
[Table("Users", Schema = AnonymousChatDbContext.PublicSchema)]
[Index(nameof(Id), IsUnique = true)]
public sealed class User
{
    [Key]
    public long Id { get; set; }
    [StringLength(maximumLength: 50, MinimumLength = 3)]
    public required string Login { get; set; }
    [StringLength(maximumLength: 50, MinimumLength = 6)]
    public required string Password { get; set; }
    public required DateTimeOffset TimeStamp { get; set; }
}