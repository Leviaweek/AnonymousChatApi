namespace AnonymousChatApi;

public static class Constants
{
    public const string CookieAccessTokenString = "accessToken";
    public const string CookieRefreshTokenString = "refreshToken";
    public const string JwtUserIdClaimType = "userId";
    public const string JwtLifeTimeClaimType = "lifeTime";
    public const string JwtCreatedAtClaimType = "cratedAt";
    public const string JwtHasRefreshTokenType = "hasRefreshToken";
    public static readonly TimeSpan RefreshTokenLifeTime = TimeSpan.FromDays(30);
    public static readonly TimeSpan AccessTokenLifeTime = TimeSpan.FromMinutes(60);

}