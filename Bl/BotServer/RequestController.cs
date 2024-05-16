using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;
using TelegramWrapper.General.Listener;
using TelegramWrapper.Helpers;
using TelegramWrapper.Models;
using TelegramWrapper.Wrapper;
using TelegramWrapper.Wrapper.Bl.Handlers;

namespace social_analytics.Bl.BotServer
{
    internal class RequestController : IUnexpectedMessagaHandler
    {
        public IClientWrapper tg { get; set; }
        private ClientServer _app;
        public RequestController(IClientWrapper clientWrapper, ClientServer app)
        {
            tg = clientWrapper;
            _app = app;
        }
        public async Task<bool> Handle(Message message)
        {
            if (tg.IsInitialized())
            {
                IChatContext chat = tg.GetOrCreateChatContextWith(message.ChatId);
                var model = ModelConverter.FromTelegramMessageToWrapped(message);
                bool sucs =  await HanldeSimilarityRequst(model);
                chat.OnAddMessage += OnRequest;
                return sucs;
            }
            return false;
        }
        private void  OnRequest(object contect, OnMessageAddEventArg args)
        {
            if (args.ReceivedMessage.SenderId != tg.UserId)
            {
                Task.Run( ()=> HanldeSimilarityRequst(args.ReceivedMessage) );
            }
        }
        private async Task<bool> HanldeSimilarityRequst(MessageModel model)
        {
            IEnumerable<MessageModel> response = (await _app.FindSimilarPosts(model.Text));
            if (response.Count() <= 0)
            {
                tg.SendMessage(model.ChatId,$"Не найдено '{model.Text}'");
            }
            foreach (var msg in response)
            {
                await tg.ForwardAndPreloadMessage(model.ChatId, msg.ChatId, false, false, false, msg.MessageId);
                await Task.Delay(100);
            }
            return true;
        }
    }
}   
