using DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainFeatures.Chats.Domain
{
    public class Chat
    {
        public Guid? ChatId { get; set; }
        public List<Message> Messages { get; set; }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        public static Chat FromDTO(ChatDTO chatDTO)
        {
            return new Chat
            {
                ChatId = chatDTO.Id,
                Messages = chatDTO.Messages.Select(x => Message.FromDTO(x)).ToList(),
            };
        }

        public ChatDTO ToDTO()
        {
            return new ChatDTO
            {
                Id = this.ChatId,
                Messages = Messages.Select(x => x.ToDTO()).ToList(),
            };
        }
    }
}
