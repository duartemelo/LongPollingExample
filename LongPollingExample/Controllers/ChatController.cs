using LongPollingExample.Data;
using LongPollingExample.Dtos;
using LongPollingExample.Entities;
using LongPollingExample.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace LongPollingExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IBaseRepository<Message> _messagesRepository;
        private readonly ApplicationDbContext _context;
        public ChatController(IBaseRepository<Message> messagesRepository, ApplicationDbContext ctx)
        {
            _messagesRepository = messagesRepository;
            _context = ctx;
        }

        [HttpGet(Name = "messages")]
        public async Task<IActionResult> GetMessages(Guid chatId, long lastMessageId)
        {
            var messages = await _messagesRepository.GetEntity().Where(x => x.ChatId == chatId && x.Id > lastMessageId).ToListAsync();

            return new ObjectResult(null)
            {
                StatusCode = (int)HttpStatusCode.OK,
                Value = messages
            };
        }

        [HttpPost(Name = "messages")]
        public async Task<IActionResult> SendMessage(MessageDto message)
        {
            var createdMessage = new Message(message.ChatId, message.Message);

            await _messagesRepository.AddAsync(createdMessage);
            await _context.SaveChangesAsync();

            return new ObjectResult(null)
            {
                StatusCode = (int)HttpStatusCode.OK,
                Value = createdMessage
            };
        }
    }
}
