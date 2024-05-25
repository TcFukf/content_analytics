using social_analytics.Bl.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;
using TelegramWrapper.Models;

namespace social_analytics.Bl.Messages
{
    public interface IMessagesService
    {
        Task<IEnumerable<MessageModel>> SearchMessages(MessageSearchOptions filter, int limit = -1);
        Task<IEnumerable<MessageModel>> GetMessages(long? messageId, long chatId, int limit = -1);
        Task<int> UpdateMessagesInRepositoryFromChats(MessageSearchOptions options, IEnumerable<long> chatId, bool fromOldest = false);
        Task<int> UpdateRepoFromChat(MessageSearchOptions options, long chatId, long fromMessageId = 0);

    }
}
