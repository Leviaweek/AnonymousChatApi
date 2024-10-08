namespace AnonymousChatApi.Models.Db;

[Serializable]
public sealed record DbUser(Ulid Id, string Login, string Password);