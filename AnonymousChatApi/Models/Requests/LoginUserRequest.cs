namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record LoginUserRequest(string Login, string Password);