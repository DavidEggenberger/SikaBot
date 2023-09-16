using DomainFeatures;
using DomainFeatures.Chats;
using DomainFeatures.Chats.Domain;
using DTOs.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ChatPersistence chatPersistence;
        private readonly ChatBotService chatBotService;
        private readonly IHubContext<NotificationHub> hubContext;
        public ChatsController(ChatPersistence chatPersistence, ChatBotService chatBotService)
        {
            this.chatPersistence = chatPersistence;
            this.chatBotService = chatBotService;
        }

        /// <summary>
        /// Retrieve a specified chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [HttpGet("{chatId}")]
        public ActionResult<ChatDTO> Get(Guid chatId)
        {
            return Ok(chatPersistence.Chats.Single(c => c.ChatId == chatId));
        }

        /// <summary>
        /// Create a new chat
        /// </summary>
        /// <param name="chat"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ChatDTO>> CreateChat(ChatDTO chatDTO)
        {
            chatDTO.Id = Guid.NewGuid();

            foreach (var messages in chatDTO.Messages)
            {
                messages.SentAt = DateTime.Now;
            }

            var chat = Chat.FromDTO(chatDTO);

            chatPersistence.AddChat(chat);

            var message = chat.Messages.Last();

            chatBotService.AnswerQuestionAsync(chat, message);

            return Ok(chat.ToDTO());
        }

        /// <summary>
        /// Add a message to an existing chat
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="messageDTO"></param>
        /// <returns></returns>
        [Tags("Messages")]
        [HttpPost("{chatId}/messages")]
        public async Task<ActionResult> CreateMessageInChat([FromRoute] Guid chatId, MessageDTO messageDTO)
        {
            messageDTO.SentAt = DateTime.Now;

            Chat chat = chatPersistence.Chats.Single(x => x.ChatId == chatId);
            Message message = Message.FromDTO(messageDTO);

            chat.AddMessage(message);

            var newmessage = chat.Messages.Last();

            chatBotService.AnswerQuestionAsync(chat, newmessage);

            return Ok();
        }
    }
}
