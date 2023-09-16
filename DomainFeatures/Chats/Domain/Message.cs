using DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.Chats.Domain
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
        public bool BotGenerated { get; set; }

        public static Message FromDTO(MessageDTO messageDTO)
        {
            return new Message
            {
                Text = messageDTO.Text,
                SentAt = messageDTO.SentAt,
                BotGenerated = messageDTO.BotGenerated,
            };
        }

        public MessageDTO ToDTO()
        {
            return new MessageDTO()
            {
                Text = Text,
                SentAt = SentAt,
                BotGenerated = BotGenerated
            };
        }
    }
}
