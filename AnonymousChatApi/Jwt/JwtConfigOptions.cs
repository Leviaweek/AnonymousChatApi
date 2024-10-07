using System.ComponentModel.DataAnnotations;

namespace AnonymousChatApi.Jwt;

public class JwtConfigOptions
{
    public const string OptionsName = "JwtConfig:Secret";
    [Required] public string SecretKey { get; set; } = string.Empty;
}