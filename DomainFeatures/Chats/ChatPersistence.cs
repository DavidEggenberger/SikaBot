using DomainFeatures.Chats.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.Chats
{
    public class ChatPersistence
    {
        public ChatPersistence()
        {
            Chats = new List<Chat>();
        }

        public List<Chat> Chats { get; set; }

        public void AddChat(Chat chat)
        {
            Chats.Add(chat);
        }
    }
}
