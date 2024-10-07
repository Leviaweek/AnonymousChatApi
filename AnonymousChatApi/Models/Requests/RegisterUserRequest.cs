namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record RegisterUserRequest(string Login, string Password);