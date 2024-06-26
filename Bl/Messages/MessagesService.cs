﻿using social_analytics.Bl.Filter;
using social_analytics.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;
using TelegramWrapper.Helpers;
using TelegramWrapper.Models;
using TelegramWrapper.TelegramParser;

namespace social_analytics.Bl.Messages
{
    public class MessagesService : IMessagesService
    {
        private readonly IClientParser _parser;
        private readonly IMessageDAL _messageDAL;
        public MessagesService(IClientParser parser, IMessageDAL messageDAL)
        {
            _parser = parser;
            _messageDAL = messageDAL;
        }
        public async Task<IEnumerable<MessageModel>> GetMessages(long? messageId, long chatId, int limit = -1)
        {
            return await _messageDAL.GetMessages(messageId, chatId, limit);
        }

        public async Task<IEnumerable<MessageModel>> SearchMessages(MessageSearchOptions filter, int limit = -1)
        {
            return await _messageDAL.SearchMessages(filter,limit);
        }
        /// <summary>
        /// return count of added to DB messages (some of them can already be in DB)
        /// </summary>
        /// <param name="options"></param>
        /// <param name="chatIds"></param>
        /// <returns></returns>
        public async Task<int> UpdateMessagesInRepositoryFromChats(MessageSearchOptions options, IEnumerable<long> chatIds, bool fromOldest)
        {
            if (options == null)
            {
                throw new ArgumentException("Cant update messages without options");
            }
            int added = 0;
            var tasks = new List<Task>();
            (long chatId, DateTime time, long msgId)[] oldestMessages = null;
            if (options.DateOptions != null && fromOldest)
            {
                oldestMessages = (await _messageDAL.OldestMessages()).Select(msg=>(msg.ChatId,msg.Date,msg.MessageId)).ToArray(); 
            }
            foreach (var chatId in chatIds)
            {
                long fromMessage = oldestMessages?.Where(x=>x.Item1 == chatId).Select(x=>x.Item3).FirstOrDefault() ?? 0;
                Console.WriteLine($"NOW ENTER IN {chatId}");
                added += await UpdateRepoFromChat(options,chatId,fromMessage);
                Console.WriteLine($"NOW END FROM {chatId}");
            }
            Task.WaitAll(tasks.ToArray());
            return added;
        }
        /// <summary>
        /// OPTIONS.TILLDATE isnt Using.
        /// UPDATE MESAGES FROM options.FromDate TILL now
        /// return count of added to DB messages (some of them may already  be in DB)
        /// </summary>
        /// <param name="options"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<int> UpdateRepoFromChat(MessageSearchOptions options, long chatId, long fromMessageId = 0)
        {
            Message[] messages;
            int addedCount = 0;
            do
            {
                messages = (await _parser.GetMessages(chatId, 100,fromMessageId:fromMessageId)).ToArray();
                if (messages.Length == 0)
                {
                    break;
                }
                if (!(await RangeInRepo(messages)))
                {
                    addedCount += messages.Length;
                    await _messageDAL.InsertMessages( messages.Select(tgm=>ModelConverter.FromTelegramMessageToWrapped(tgm)).ToArray() );
                }
                fromMessageId = messages.Last().Id;
            }
            while (LastIsLaterThan(messages, options.DateOptions.FromDate));
            return addedCount;

        }

        private async Task<bool> RangeInRepo(Message[] messages)
        {
            Message first = messages.First();
            Message last = messages.Last();

            MessageModel firstRepo = (await _messageDAL.GetMessages(first.Id,first.ChatId,1)).SingleOrDefault();
            if (firstRepo == null)
            {
                return false;
            }
            MessageModel lastRepo = (await _messageDAL.GetMessages(last.Id, last.ChatId, 1)).SingleOrDefault();
            return lastRepo != null;


        }

        private bool LastIsLaterThan(Message[] messages, DateTime? fromDate)
        {
            long unixSec = ((DateTimeOffset)fromDate.Value.Date).ToUnixTimeSeconds();
            return messages.LastOrDefault()?.Date > unixSec;
        }
    }
    
}
