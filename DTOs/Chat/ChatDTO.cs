using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Chat
{
    public class ChatDTO
    {
        public Guid? Id { get; set; }
        public List<MessageDTO> Messages { get; set; }
    }
}
