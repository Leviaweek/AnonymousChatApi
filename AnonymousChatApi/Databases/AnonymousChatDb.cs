using System.Collections.Concurrent;
using AnonymousChatApi.Models.Db;
using EventHandler = AnonymousChatApi.Services.EventHandler;

namespace AnonymousChatApi.Databases;

//ToDo be sure to replace with ef core
public sealed class AnonymousChatDb(ILogger<AnonymousChatDb> logger, EventHandler handler)
{
    private readonly Dictionary<Ulid, DbUser> _users = [];
    private readonly ConcurrentDictionary<Ulid, DbChat> _chats = [];
    private readonly Dictionary<Ulid, DbChatMessage> _messages = [];
    
    private readonly Dictionary<Ulid, HashSet<Ulid>> _chatUsers = [];
    
    private readonly ConcurrentDictionary<Ulid, HashSet<Ulid>> _chatMessagesIndex = [];
    public async Task<DbChatMessage?> AddMessageAsync(DbChatMessage message, CancellationToken cancellationToken)
    {
         message = message with { Id = Ulid.NewUlid() };

        if (!_users.TryGetValue(message.SenderId, out var user))
            return null;

        if (!_chatUsers.TryGetValue(message.ChatId, out var users))
            return null;

        if (!users.Contains(user.Id))
            return null;
        
        _messages.Add(message.Id, message);
        _chatMessagesIndex.AddOrUpdate(message.ChatId, static (_, messageId) => [messageId],
            static (_, set, messageId) =>
        { 
            set.Add(messageId);
            return set;
        }, message.Id);
        
        logger.LogInformation("Added message: {message}", message);

        foreach (var chatUserId in users)
        {
            await handler.OnNewMessageAsync(chatUserId, message.ToDto(), cancellationToken);
        }
        
        return message;
    }
    
    public List<DbChatMessage>? GetMessages(Ulid chatId, Ulid userId)
    {
        if (!_chatUsers.TryGetValue(chatId, out var users))
            return null;

        if (!users.Contains(userId))
            return null;

        if (!_chats.TryGetValue(chatId, out var chat))
        {
            _chatUsers.Remove(chatId);
            return null;
        }

        if (!_chatMessagesIndex.TryGetValue(chat.Id, out var messages))
        {
            messages = [];
            _chatMessagesIndex.TryAdd(chat.Id, messages);
        }

        var resultCollection = new List<DbChatMessage>();
        foreach (var messageId in messages)
        {
            if (!_messages.TryGetValue(messageId, out var message))
            {
                _chatMessagesIndex[chatId].Remove(messageId);
                continue;
            }
            resultCollection.Add(message);
        }

        resultCollection.Sort((previousMessage, currentMessage) => previousMessage.Id.CompareTo(currentMessage.Id));

        return resultCollection;
    }

    public DbChat AddChat(string name)
    {
        var id = Ulid.NewUlid();
        var chat = new DbChat(id, name, DateTimeOffset.UtcNow);
        _chats.TryAdd(id, chat);
        return chat;
    }

    public async Task<bool> AddUserToChatAsync(Ulid userId, Ulid chatId, CancellationToken cancellationToken)
    {
        if (!_users.ContainsKey(userId))
            return false;

        if (!_chats.ContainsKey(chatId))
            return false;

        if (!_chatUsers.TryGetValue(chatId, out var users))
        {
            users = [];
            _chatUsers.Add(chatId, users);
        }

        foreach (var user in users)
        {
            await handler.OnUserJoinAsync(user, chatId, cancellationToken);
        }
        
        return users.Add(userId);
    }

    public DbUser AddUser(string login, string password)
    {
        var id = Ulid.NewUlid();
        var user = new DbUser(id, login, password);
        _users.Add(id, user);
        return user;
    }

    public bool ContainsUser(string login)
    {
        foreach (var (_, user) in _users)
            if (user.Login == login)
                return true;

        return false;
    }
    
    public DbUser? GetUser(string login, string password)
    {
        foreach (var (_, user) in _users)
            if (user.Login == login && user.Password == password)
                return user;

        return null;
    }

    public DbChat GetRandomChat()
    {
        var random = new Random();
        if (_chats.IsEmpty)
        {
            return AddChat("RandomChat");
        }
        var chatIndex = random.Next(_chats.Count);
        var chat = _chats.Values.Skip(chatIndex).First();
        return chat;
    }

    public DbUser? GetUserById(Ulid id) => _users.GetValueOrDefault(id);
}