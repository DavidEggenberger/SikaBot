using DomainFeatures.Chats.Domain;
using DomainFeatures.OpenAi;
using DomainFeatures.QuestionAnswering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainFeatures.Chats
{
    public class ChatBotService
    {
        private readonly OpenAIService openAIService;
        private readonly QuestionAnswererService questionAnswererService;
        private readonly IHubContext<NotificationHub> hubContext;
        public ChatBotService(OpenAIService openAIService, IHubContext<NotificationHub> hubContext, QuestionAnswererService questionAnswererService)
        {
            this.openAIService = openAIService;
            this.hubContext = hubContext;
            this.questionAnswererService = questionAnswererService;
        }

        public async Task AnswerQuestionAsync(Chat chat, Message message)
        {


            var response = await openAIService.MakeOpenAIRequest(message.Text);

            chat.Messages.Add(new Message { Text = response, BotGenerated = true });

            await hubContext.Clients.All.SendAsync("Refresh");
        }
    }
}
