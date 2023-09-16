using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Chat
{
    public class MessageDTO
    {
        public DateTime SentAt { get; set; }
        public string Text { get; set; }
        public bool BotGenerated { get; set; }
    }
}
