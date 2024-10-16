using AnonymousChatApi.Databases.Models;
using AnonymousChatApi.Models.Dtos;
using AnonymousChatApi.Models.Events;
using Bogus;
using Microsoft.EntityFrameworkCore;
using EventHandler = AnonymousChatApi.Services.EventHandler;

namespace AnonymousChatApi.Databases;

public sealed class AnonymousChatDb(
    ILogger<AnonymousChatDb> logger,
    EventHandler handler,
    IDbContextFactory<AnonymousChatDbContext> contextFactory)
{
    public async ValueTask<MessageDto?> AddTextMessageAsync(MessageDto message, CancellationToken cancellationToken)
    {
        if (message.TextMessage is null)
            return null;
        
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        var user = await db.Users
            .FirstOrDefaultAsync(user => user.Id == message.SenderId,
            cancellationToken: cancellationToken);

        if (user is null)
            return null;
        
        var chat = await db.Chats.Include(chat => chat.ChatUsers)
            .FirstOrDefaultAsync(chat => chat.Id == message.ChatId,
            cancellationToken: cancellationToken);

        var userChat = chat?.ChatUsers.FirstOrDefault(x => x.ChatId == message.ChatId);
        
        if (chat is null || userChat is null)
            return null;

        var dbMessage = new MessageBase
        {
            ChatId = message.ChatId,
            TimeStamp = DateTimeOffset.UtcNow,
            UserId = message.SenderId,
            TextMessage = new TextMessage
            {
                Text = message.TextMessage.Text
            },
            NotifyMessage = null,
            IsDeleted = false
        };

        var result = await db.MessageBases.AddAsync(dbMessage, cancellationToken: cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        message = result.Entity.ToDto();
        logger.LogInformation("Added message: {message}", message);
        
        foreach (var chatUser in chat.ChatUsers)
        {
            var @event = new NewMessageEvent(message);
            await handler.OnEventAsync(chatUser.UserId, @event, cancellationToken);
        }
        
        return message;
    }
    
    public async ValueTask<List<MessageDto>?> GetMessages(long chatId,
        long userId,
        long startId,
        bool asc,
        int count,
        CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        var chatUser = await db.ChatUsers
            .Where(chatUser => !chatUser.Chat.IsDeleted)
            .FirstOrDefaultAsync(chatUser => chatUser.UserId == userId && chatUser.ChatId == chatId,
            cancellationToken: cancellationToken);

        if (chatUser is null)
            return null;

        var messages = db.MessageBases
            .Where(m => m.ChatId == chatId)
            .Where(m => !m.IsDeleted)
            .OrderByDescending(m => m.Id)
            .AsQueryable();

        if (asc)
            messages = messages
                .Where(m => m.Id < startId);
        else
            messages = messages
                .Where(m => m.Id > startId);
        
        var result = await messages.Take(count)
            .Include(m => m.TextMessage)
            .Include(m => m.NotifyMessage)
            .AsSingleQuery()
            .Select(message => message.ToDto())
            .ToListAsync(cancellationToken: cancellationToken);
        
        return result;
    }

    public async ValueTask<ChatDto> AddChatAsync(string name, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        var chat = new Chat
        {
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow,
            ChatUsers = [],
            Messages = [],
            IsDeleted = false
        };

        var result = await db.Chats.AddAsync(chat, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var dto = result.Entity.ToDto();

        return dto;
    }

    public async ValueTask<bool> AddUserToChatAsync(long userId, long chatId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = await db.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken: cancellationToken);

        if (user is null)
            return false;

        var chat = await db.Chats
            .Include(chat => chat.ChatUsers)
            .AsSingleQuery()
            .FirstOrDefaultAsync(chat => chat.Id == chatId,
                cancellationToken: cancellationToken);

        if (chat is null)
            return false;

        var chatUser = chat.ChatUsers.FirstOrDefault(x => x.UserId == user.Id);

        if (chatUser is not null)
            return false;

        var newChatUser = new ChatUser
        {
            ChatId = chat.Id,
            UserId = user.Id,
            LastReadMessageId = null,
            User = user,
            Chat = chat,
            UserName = new Faker().Name.FullName()
        };
        
        chat.ChatUsers.Add(newChatUser);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var eventUser in chat.ChatUsers)
        {
            var @event = new UserJoinEvent(new UserJoinDto(eventUser.UserId, chatId, DateTimeOffset.UtcNow));
            await handler.OnEventAsync(eventUser.UserId, @event, cancellationToken);
        }

        return true;
    }

    public async ValueTask<UserDto?> AddUserAsync(string login, string password, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var dbUser = await db.Users.FirstOrDefaultAsync(user => user.Login == login, cancellationToken: cancellationToken);

        if (dbUser is not null)
            return null;
        
        var user = new User
        {
            Login = login,
            Password = password,
            TimeStamp = DateTimeOffset.UtcNow
        };

        var result = await db.Users.AddAsync(user, cancellationToken: cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        
        var dto = result.Entity.ToDto();
        return dto;
    }

    public async Task<UserDto?> GetUserAsync(string login, string password, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = await db.Users.FirstOrDefaultAsync(user => user.Login == login && user.Password == password,
            cancellationToken);
        
        return user?.ToDto();
    }

    public async ValueTask<ChatDto> GetRandomChatAsync(long userId, CancellationToken cancellationToken)
    {
        var random = new Random();

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var chatsWithoutUser = db.ChatUsers.Where(chatUser => chatUser.UserId != userId);
        
        var count = await chatsWithoutUser.CountAsync(cancellationToken: cancellationToken);
        
        if (count is 0)
        {
            var name = new Faker().Random.String(16);
            return await AddChatAsync(name, cancellationToken);
        }
        var chatIndex = random.Next(count);
        var chat = chatsWithoutUser.Skip(chatIndex).First().Chat;
        
        ArgumentNullException.ThrowIfNull(chat);
        
        return chat.ToDto();
    }

    public async ValueTask<UserDto?> GetUserByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = await db.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken: cancellationToken);

        return user?.ToDto();
    }

    public async ValueTask<bool> TryReadMessagesAsync(long userId, long chatId, long lastReadId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var result = await db.ChatUsers
            .Where(user => user.UserId == userId)
            .Where(user => user.ChatId == chatId)
            .Where(user => !user.Chat.IsDeleted)
            .Where(user => user.LastReadMessageId == null || user.LastReadMessageId < lastReadId)
            .Where(user => user.Chat.Messages.Any(m => m.Id == lastReadId))
            .ExecuteUpdateAsync(u => u.SetProperty(c => c.LastReadMessageId, lastReadId),
                cancellationToken);

        return result > 0;
    }

    public async ValueTask<int?> GetUpdatesCountAsync(long chatId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var users = db.ChatUsers
            .Where(user => user.Chat.Id == chatId)
            .Where(user => !user.Chat.IsDeleted);

        var user = await users.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user is null)
            return null;
        
        var count = await users
            .SelectMany(chatUser => chatUser.Chat.Messages)
            .Where(m => !m.IsDeleted)
            .Where(m => m.Id > user.LastReadMessageId)
            .CountAsync(cancellationToken: cancellationToken);
        
        return count;
    }

    public async ValueTask<List<ChatDto>> GetAllUserChatsAsync(long userId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var chats = db.ChatUsers
            .Where(chatUser => chatUser.UserId == userId)
            .Select(chatUser => chatUser.Chat)
            .Include(chat => chat.ChatUsers);

        return await chats.Select(chat => chat.ToDto()).ToListAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> DeleteChatAsync(long chatId, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var result = await db.Chats.Where(chat => chat.Id == chatId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return result == 1;
    }
}