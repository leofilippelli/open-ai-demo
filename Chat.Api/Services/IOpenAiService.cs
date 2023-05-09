using Chat.Api.Models;

namespace Chat.Api.Services;

public interface IOpenAiService
{
    Task<Conversation> CreateConversation(string? id);
    
    Task<Conversation[]> GetConversations();
    
    Task<Conversation?> GetConversation(string id);
    
    Task DeleteConversation(string id);
    
    Task AddMessage(string conversationId, string text);
}