using LongPollingExample.Data;
using LongPollingExample.Dtos;
using LongPollingExample.Entities;
using LongPollingExample.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LongPollingExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IBaseRepository<Message> _messagesRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memCache;
        public ChatController(IBaseRepository<Message> messagesRepository, ApplicationDbContext ctx, IMemoryCache memCache)
        {
            _messagesRepository = messagesRepository;
            _context = ctx;
            _memCache = memCache;
        }

        [HttpGet(Name = "messages")]
        public async Task<IActionResult> GetMessages(Guid chatId, long lastMessageId)
        {
            // Obter maior lastMessageId da cache
            // Se não existe, coloca a 0 na cache
            string cacheKey = $"LastMessageId_{chatId}";
            long cachedLastMessageId = _memCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return (long)0; // Default se não existir
            });

            // Se o maior lastMessageId da cache é superior ao lastMessageId fornecido por parametro, obtem as mensagens
            if (cachedLastMessageId > lastMessageId || lastMessageId == 0)
            {
                var newMessages = await GetNewMessages(chatId, lastMessageId);
                UpdateCacheWithHighestMessageId(cacheKey, newMessages.Max(x => x.Id));
                return Ok(newMessages);
            }

            // Long polling ate haver uma nova mensagem
            DateTime startTime = DateTime.UtcNow;
            TimeSpan timeout = TimeSpan.FromSeconds(100);

            while ((DateTime.UtcNow - startTime) < timeout)
            {
                // Obter novamente da cache, no caso de existirem novas mensagens
                cachedLastMessageId = _memCache.Get<long>(cacheKey);

                if (cachedLastMessageId > lastMessageId) // Existe nova mensagem
                {
                    var newMessages = await GetNewMessages(chatId, lastMessageId);
                    UpdateCacheWithHighestMessageId(cacheKey, newMessages.Max(x => x.Id));
                    return Ok(newMessages);
                }

                await Task.Delay(50); // Aguarda antes de tentar novamente
            }

            return NoContent();
        }

        [HttpPost(Name = "messages")]
        public async Task<IActionResult> SendMessage(MessageDto message)
        {
            string cacheKey = $"LastMessageId_{message.ChatId}";
            var createdMessage = new Message(message.ChatId, message.Message);

            await _messagesRepository.AddAsync(createdMessage);
            await _context.SaveChangesAsync();

            UpdateCacheWithHighestMessageId(cacheKey, createdMessage.Id);

            return Ok(createdMessage);
        }

        private async Task<List<Message>> GetNewMessages(Guid chatId, long lastMessageId)
        {
            return await _messagesRepository.GetEntity().Where(x => x.ChatId == chatId && x.Id > lastMessageId).ToListAsync();
        }

        private void UpdateCacheWithHighestMessageId(string cacheKey, long messageId)
        {
            // TODO: validate if there is already a bigger ID in cache (?) not needed for demo purposes :D
            _memCache.Set(cacheKey, messageId, TimeSpan.FromMinutes(10));
            
        }
    }
}
