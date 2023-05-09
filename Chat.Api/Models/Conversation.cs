namespace Chat.Api.Models;

public class Conversation
{
    public string Id { get; set; } = string.Empty;
    public List<Message> Messages { get; set; } = new ();
}