namespace AnonymousChatApi.Models;

[Serializable]
public sealed record User(Ulid Id, string Login, string Password);