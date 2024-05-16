using social_analytics.Bl.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramWrapper.DAL;
using TelegramWrapper.Models;

namespace social_analytics.DAL
{
    public interface IMessageDAL : IStoreData
    {
        Task InsertMessages(params MessageModel[] messages);
        Task<IEnumerable<MessageModel>> GetMessages(long? messageId,long chatId, int limit = -1);
        Task<IEnumerable<MessageModel>> OldestMessages();
        Task<IEnumerable<MessageModel>> SearchMessages(MessageSearchOptions filter, int limit = -1);
    }
}
