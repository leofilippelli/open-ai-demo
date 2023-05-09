using System.Data;
using Chat.Api.Models;
using Chat.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IOpenAiService _service;

    public ConversationsController(IOpenAiService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<ActionResult<Conversation>> CreateConversation([FromQuery] string? id)
    {
        var conversation = await _service.CreateConversation(id);
        return Ok(conversation);
    }

    [HttpGet]
    public async Task<ActionResult<Conversation[]>> GetConversations()
    {
        return await _service.GetConversations();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Conversation[]>> GetConversation([FromRoute] string id)
    {
        var conversation = await _service.GetConversation(id);
        if (conversation is null)
        {
            return NotFound();
        }
        
        return Ok(conversation);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<Conversation[]>> DeleteConversation([FromRoute] string id)
    {
        try
        {
            await _service.DeleteConversation(id);
            return Ok();
        }
        catch (DataException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpPost("{conversationId}")]
    public async Task<ActionResult> AddMessage([FromRoute] string conversationId, [FromBody] string text)
    {
        try
        {
            await _service.AddMessage(conversationId, text);
            return Ok();
        }
        catch (DataException ex)
        {
            return NotFound(ex.Message);
        }
    }
}