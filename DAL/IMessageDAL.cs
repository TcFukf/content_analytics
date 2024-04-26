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
        Task<IEnumerable<MessageModel>> GetMessages(MessageModel msg, int limit = -1, MessageFilterOptions? filter = null);
    }
}
