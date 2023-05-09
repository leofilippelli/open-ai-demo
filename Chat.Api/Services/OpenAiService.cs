using System.Data;
using Chat.Api.Config;
using Chat.Api.Models;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Chat.Api.Services;

public class OpenAiService : IOpenAiService
{
    private readonly RestClient _client;
    
    private readonly OpenAiServiceConfig _config;
    private readonly List<Conversation> _storedConversations = new ();

    public OpenAiService(IOptions<OpenAiServiceConfig> config)
    {
        _config = config.Value;
        _client = new RestClient(_config.Endpoint);
    }
    
    public async Task<Conversation> CreateConversation(string? id)
    {
        var newId = !string.IsNullOrWhiteSpace(id) ? id : DateTime.Now.ToShortTimeString();
        
        var existing = await GetConversation(newId);
        if (existing is not null)
        {
            throw new DataException($"Conversation with id {id} already exists");
        }
        
        var conversation = new Conversation
        {
            Id = newId,
            Messages = new List<Message>()
        };
        
        _storedConversations.Add(conversation);
        return conversation;
    }

    public Task<Conversation[]> GetConversations()
    {
        return Task.FromResult(_storedConversations.ToArray());
    }
    
    public Task<Conversation?> GetConversation(string id)
    {
        return Task.FromResult(_storedConversations.FirstOrDefault(x => x.Id == id));
    }

    public async Task DeleteConversation(string id)
    {
        var conversation = await GetConversation(id);
        if (conversation is null)
        {
            throw new DataException("Conversation not found");
        }
        
        _storedConversations.Remove(conversation);
    }

    public async Task AddMessage(string conversationId, string text)
    {
        var currentConversation = await GetConversation(conversationId);
        if (currentConversation is null)
        {
            throw new DataException("Conversation not found");
        }

        var messagesToSend = new List<Message>();
        currentConversation.Messages.ToList().ForEach(x => messagesToSend.Add(x));

        var newUserMessage = new Message
        {
            Role = "user",
            Content = text
        };
        messagesToSend.Add(newUserMessage);
        
        var body = new ChatCompletionRequest
        {
            Model = _config.Model,
            Messages = messagesToSend.ToArray()
        };
        
        var response = await SendApiMessageAsync(body);
        if (response is null || !response.Choices.Any())
        {
            throw new InvalidOperationException($"Not able to create new message for conversation {currentConversation.Id}");
        }
        
        // update in-memory storage
        currentConversation.Messages.Add(newUserMessage);
        currentConversation.Messages.Add(response.Choices.First().Message);
    }

    private async Task<ChatCompletionResponse?> SendApiMessageAsync(ChatCompletionRequest body)
    {
        var request = new RestRequest("chat/completions").AddJsonBody(body);
        //request.AddHeader("Authorization", "Bearer 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");
        request.AddHeader("Authorization", $"Bearer {_config.ApiKey}");
        
        var response = await _client.ExecutePostAsync<ChatCompletionResponse>(request);
        return response.IsSuccessful ? response.Data : null;
    }
}