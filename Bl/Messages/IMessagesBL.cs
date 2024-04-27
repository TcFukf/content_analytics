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
    public interface IMessagesBL
    {
        Task<IEnumerable<MessageModel>> SearchMessages(MessageSearchOptions filter, int limit = -1);
        Task<IEnumerable<MessageModel>> GetMessage(long messageId, long chatId, int limit = -1);
        Task<int> UpdateMessagesInRepositoryFromChats(MessageSearchOptions options, IEnumerable<long> chatId);
        Task <int> UpdateRepoFromChat(MessageSearchOptions options, long chatId);

    }
}
