﻿using social_analytics.Bl.Filter;
using social_analytics.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;
using TelegramWrapper.Models;

namespace social_analytics.DAL
{
    //public long MessageId { get; set; }
    //public PlatformId? PlatrormId { get; set; } = null!;
    //public long SenderId { get; set; }
    //public long ChatId { get; set; }
    //public DateTime Date { get; set; }
    //public string Text { get; set; }
    //public long? RepliedId { get; set; }
    //public long? StickerId { get; set; }
    public class MessageDAL : IMessageDAL
    {
        public async Task InsertMessages(params MessageModel[] messages)
        {
            MessageModel emptyModel = new MessageModel();
            string sql = $"""
                         insert into message(PlatformId,MessageId,SenderId,ChatId,"Date","Text",RepliedId,StickerId)
                         VALUES
                         (@{nameof(emptyModel.PlatFormId)},@{nameof(emptyModel.MessageId)},@{nameof(emptyModel.SenderId)}, @{nameof(emptyModel.ChatId)}, @{nameof(emptyModel.Date)}, @{nameof(emptyModel.Text)}, @{nameof(emptyModel.RepliedId)}, @{nameof(emptyModel.StickerId)}   )
                         on conflict (messageId,chatId) do nothing ;
                         """;
            await DbHelper.ExecuteAsync(sql, messages);
        }
        public async Task<IEnumerable<MessageModel>> GetMessages(MessageModel msg, int limit = -1, MessageFilterOptions? filter = null)
        {
            MessageModel emptyModel = new MessageModel();
            string limitLine = $"limit {limit}";
            StringBuilder sql = new StringBuilder(
                         $"""
                         select * from message where  (messageId,ChatId) = (@{nameof(emptyModel.MessageId)},@{nameof(emptyModel.ChatId)})
                         """
                                                 );
            if (filter != null)
            {
                if (filter.From != null)
                {
                    string dateFilterLine = $"""
                        where " {filter.From}<= Date and <= {filter.Till}"
                        """;
                    sql.AppendLine(dateFilterLine);
                }
            }
            if (limit != -1)
            {
                sql.AppendLine(limitLine);
            }
            return await DbHelper.Query<MessageModel>(sql.ToString(),msg);
        }

        public async Task WriteEnumerable(IEnumerable<object> objects, Type objectType, string separator = "\n")
        {
            if (objectType != typeof(MessageModel))
            {
                throw new ArgumentException("cant write another type");
            }
            await InsertMessages(objects.Select(obj => obj as MessageModel)?.ToArray());

        }
    }
}
