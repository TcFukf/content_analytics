using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer;
using social_analytics.Bl.TextAnalytics.TextClassifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Telegram.Td.Api;
using TelegramWrapper.General.Listener;
using TelegramWrapper.Models;
using TelegramWrapper.Wrapper;

namespace social_analytics.Bl.BotServer
{
    public class NewsPoster
    {
        private Queue<TextCategory<MessageModel>> _texts;
        private ITextAnalyzer _analyzer;
        private IClientWrapper _client;
        private List<long> channelIds;
        private List<IChatContext> _listeningChannels;
        public NewsPoster(ITextAnalyzer analyzer, IClientWrapper client,List<long> chatSend)
        {
            _texts = new();
            _analyzer = analyzer;
            _client = client;
            _listeningChannels = new();
            channelIds = chatSend;
        }
        private void UpdateCategories(TextCategory<MessageModel> category, MessageModel message, WordTagsVector vector)
        {
            if (category != null)
            {
                category.Values.Add(message);
            }
            else
            {
                _texts.Enqueue(new TextCategory<MessageModel>() { Values = new() { message }, HeadVector = vector });
            }
            PostHotCategoriesAndDequeue().Wait();
            DequeueLowRateCategories();
        }
        private void DequeueLowRateCategories()
        {
            int seen = 0;

            while (_texts?.Count > 0 && seen < _texts.Count)
            {
                var category = _texts.Dequeue();
                seen++;
                if (!CategotyIsLowRate(category))
                {
                    _texts.Enqueue(category);
                }
            }
        }
        private async Task PostHotCategoriesAndDequeue()
        {
            int seen = 0;

            while(_texts?.Count > 0 && seen < _texts.Count)
            {
                var category = _texts.Dequeue();
                seen++;
                if (CategoryIsHot(category))
                {
                    await PostCategoty(category);
                }
                else
                {
                    _texts.Enqueue(category);
                }
            }
        }
        private bool CategotyIsLowRate(TextCategory<MessageModel> category)
        {
            DateTime addedTime = category.Values.MinBy(msg => msg.Date).Date;
            var stamp = addedTime - DateTime.UtcNow;
            return category.Values.Count() < 2 && stamp.Minutes > 30;
        }
        private bool CategoryIsHot(TextCategory<MessageModel> category)
        {

            DateTime addedTime = category.Values.MinBy(msg=>msg.Date).Date;
            var stamp = addedTime - DateTime.UtcNow;
            return category.Values.Count() >= 2;
        }
        private async Task PostCategoty(TextCategory<MessageModel> category)
        {
            foreach (var msg in category.Values)
            {
                foreach (var id in channelIds)
                {
                    _client.ForwardMessage(id,msg.ChatId,false,false,false,msg.MessageId);

                    await Task.Delay(1000);
                }
            }
        }
        public void ListenContext(IChatContext context)
        {
            context.OnAddMessage += OnAddMessage;
            _listeningChannels.Add(context);
        }
        public void StopListen()
        {
            foreach (var context in _listeningChannels)
            {
                context.OnAddMessage -= OnAddMessage;
            }
        }
        private void OnAddMessage(object context,OnMessageAddEventArg arg)
        {
            if (arg.ReceivedMessage == null)
            {
                return;
            }
            Console.WriteLine($"ADDED {arg.ReceivedMessage.ChatId}");
            TextCategory<MessageModel> categoryToAdd = null;
            double maxSimil = 0;
            var vect = _analyzer.CreateVector(arg.ReceivedMessage.Text);
            foreach (var category in _texts)
            {
                double simil = _analyzer.VectorSimilarity(category.HeadVector, vect);
                if (simil > maxSimil)
                {
                    maxSimil = simil;
                    categoryToAdd = category;   
                }
            }
            UpdateCategories(categoryToAdd, arg.ReceivedMessage,vect);
        }
    }
}
