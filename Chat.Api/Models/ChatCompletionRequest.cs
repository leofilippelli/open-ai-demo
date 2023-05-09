namespace Chat.Api.Models;

public class ChatCompletionRequest
{
    public string Model { get; set; } = string.Empty;
    public Message[] Messages { get; set; } = Array.Empty<Message>();
}