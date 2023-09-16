using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.OpenAi
{
    public class OpenAIService
    {
        private readonly IConfiguration configuration;
        public OpenAIService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> MakeOpenAIRequest(string prompt)
        {
            var api = new OpenAI_API.OpenAIAPI(configuration["OpenAI"]);
            var aiChat = api.Chat.CreateConversation();

            aiChat.AppendSystemMessage("You are representing Sika, a big swiss company with various different building materials. Please only" +
                "answer with short and precise words");

            aiChat.AppendUserInput(prompt);

            return await aiChat.GetResponseFromChatbotAsync();
        }
    }
}
