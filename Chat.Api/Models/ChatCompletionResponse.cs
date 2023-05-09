using System.Runtime.InteropServices.JavaScript;

namespace Chat.Api.Models;

public class ChatCompletionResponse
{
    public Choice[] Choices { get; set; } = Array.Empty<Choice>();
}

public class Choice
{
    public Message Message { get; set; }
}

