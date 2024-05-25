using Microsoft.VisualBasic;
using social_analytics.Bl.Filter;
using social_analytics.Bl.Messages;
using social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TelegramWrapper.Models;
using TelegramWrapper.Wrapper;
using TelegramWrapper.Wrapper.Bl.Handlers;

namespace social_analytics.Bl.BotServer
{
    public class ClientServer
    {
        private IMessagesService _messageService;
        private TextAnalyzer _textAnalyzer;
        private IClientWrapper _client;
        private IUnexpectedMessagaHandler _requestHandler;
        public bool IsClosed { get; set; }
        public ClientServer(IMessagesService messageService, TextAnalyzer textAnalyzer,IClientWrapper client)
        {
            _messageService = messageService;
            _textAnalyzer = textAnalyzer;
            _client = client;
            _requestHandler = new RequestController(client,this);
        }
        public async Task ListenRequests()
        {
            _client.SetUnexpectedMessagesHandler(_requestHandler);
            while (!IsClosed)
            {
                await Task.Delay(200);
            }
        }
        public async Task<int> FindingNewMessages(int milliSecPeriod = 30*60*1000)
        {
            int count = 0;
            MessageSearchOptions filter = new()
            {
                DateOptions = new()
                {
                    TillDate = DateTime.UtcNow.Date
                }
            };
            while (!IsClosed)
            {
                filter.DateOptions.FromDate = DateTime.UtcNow.AddDays(-1).Date;
                await _messageService.UpdateMessagesInRepositoryFromChats(filter, _client.GetNotPrivateChats().Result.Select(chat=>chat.Id)) ;
                await Task.Delay(milliSecPeriod);
            }
            return count;
        }
        public async Task<IEnumerable<MessageModel> > FindSimilarPosts(string text)
        {
            List< (long chatId,long messagId)> result = new();
            string[] words = _textAnalyzer.GetWords(text).ToArray();
            MessageSearchOptions filter = new MessageSearchOptions()
            {
                DateOptions = new DateOptions()
                {
                    FromDate = DateTime.UtcNow.AddDays(-10),
                    TillDate = DateTime.UtcNow,
                    DateOrdered = Order.Desc
                },
                SimilarityOptions = new SimilarityOptions() { SimilarityWords = words}
            };
            var messages = await _messageService.SearchMessages(filter, limit: 10);
            return messages = messages.OrderBy(msg => -1 * RatePost(msg.Date,(long)_textAnalyzer.CreateVector(msg.Text).GroupSumTagsOfVector) );
        }
        private long RatePost(DateTime date, long textWeight)
        {
            long time = ((DateTimeOffset)date.Date).ToUnixTimeSeconds();
            return (time*textWeight);
        }

    }
}
