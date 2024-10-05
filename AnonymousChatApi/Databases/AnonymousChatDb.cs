using System.Collections.Concurrent;
using AnonymousChatApi.Models;
using AnonymousChatApi.Services;

namespace AnonymousChatApi.Databases;

public sealed class AnonymousChatDb(ILogger<AnonymousChatDb> logger, EventMessageHandler messageHandler)
{
    private readonly Dictionary<Ulid, User> _users = [];
    private readonly ConcurrentDictionary<Ulid, Chat> _chats = [];
    private readonly Dictionary<Ulid, ChatMessage> _messages = [];
    
    private readonly Dictionary<Ulid, HashSet<Ulid>> _chatUsers = [];
    
    private readonly ConcurrentDictionary<Ulid, HashSet<Ulid>> _chatMessagesIndex = [];
    public async Task<ChatMessage?> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken)
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
            await messageHandler.OnNewMessageAsync(chatUserId, message, cancellationToken);
        }
        
        return message;
    }
    
    public List<ChatMessage>? GetMessages(Ulid chatId, Ulid userId)
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

        var resultCollection = new List<ChatMessage>();
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

    public Chat AddChat(string name)
    {
        var id = Ulid.NewUlid();
        var chat = new Chat(id, name, DateTimeOffset.UtcNow);
        _chats.TryAdd(id, chat);
        return chat;
    }

    public bool AddUserToChat(Ulid userId, Ulid chatId)
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

        return users.Add(userId);
    }

    public User AddUser(string login, string password)
    {
        var id = Ulid.NewUlid();
        var user = new User(id, login, password);
        _users.Add(id, user);
        return user;
    }

    public User? GetUser(string login, string password)
    {
        foreach (var (id, user) in _users)
            if (user.Login == login && user.Password == password)
                return user;

        return null;
    }

    public Chat GetRandomChat()
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

    public User? GetUserById(Ulid id) => _users.GetValueOrDefault(id);
}