using social_analytics.Bl.Filter;
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
        public async Task<IEnumerable<MessageModel>> GetMessages(long? messageId,long chatId, int limit = -1)
        {
            MessageModel emptyModel = new MessageModel();
            string limitLine = $"limit {limit}";
            StringBuilder sql = new StringBuilder();
            if (messageId != null)
            {
                sql.Append($"""
                         select * from message where  (messageId,ChatId) = (@{nameof(emptyModel.MessageId)},@{nameof(emptyModel.ChatId)})
                         """);
            }
            else
            {
                sql.Append($"""
                         select * from message where  ChatId = @{nameof(emptyModel.ChatId)}
                         """);
            }
            if (limit != -1)
            {
                sql.AppendLine(limitLine);
            }
            return await DbHelper.Query<MessageModel>(sql.ToString(),new {MessageId = messageId,ChatId= chatId});
        }

        public async Task WriteEnumerable(IEnumerable<object> objects, Type objectType, string separator = "\n")
        {
            if (objectType != typeof(MessageModel))
            {
                throw new ArgumentException("cant write another type");
            }
            await InsertMessages(objects.Select(obj => obj as MessageModel)?.ToArray());

        }

        public async Task<IEnumerable<MessageModel>> SearchMessages(MessageSearchOptions filter, int limit = -1)
        {
            string limitLine = $"limit {limit}";
            StringBuilder sql = new StringBuilder(
                         $"""
                         select * from message{"\n"}
                         """
                                                 );
            if (filter != null)
            {
                string whereLine = "where ";
                bool wasWhereLine = false;
                if (filter.DateOptions != null)
                {
                    sql.AppendLine(whereLine);
                    string dateFilterLine = $"""
                        @{nameof(filter.DateOptions.FromDate)} <= "Date" and "Date" < @{nameof(filter.DateOptions.TillDate)}
                        """;
                    sql.AppendLine(dateFilterLine);
                    wasWhereLine = true;
                }
                if (filter.SimilarityOptions != null)
                {
                    if (wasWhereLine && filter.SimilarityOptions.SimilarityWords.Length > 0)
                    {
                        sql.AppendLine(" AND ");
                    }
                    else
                    {
                        sql.AppendLine(whereLine);
                    }
                    int index = 0;
                    string language = filter.SimilarityOptions.Language ?? "russian";
                    for (index = 0; index < filter.SimilarityOptions.SimilarityWords.Length-1; index++)
                    {
                        string wordSim = filter.SimilarityOptions.SimilarityWords[index];
                        sql.AppendLine($"""to_tsvector('{language}', "{filter.SimilarityOptions.FieldName}") @@ to_tsquery('{language}', '{wordSim}')""");
                        sql.AppendLine(" AND ");
                    }
                    sql.AppendLine($"""to_tsvector('{language}', "{filter.SimilarityOptions.FieldName}") @@ to_tsquery('{language}', '{filter.SimilarityOptions.SimilarityWords.Last()}')""");
                }
                if (filter.ChatIds != null && filter.ChatIds.Length > 0)
                {
                    if (wasWhereLine)
                    {
                        sql.AppendLine(" AND ");
                    }
                    else
                    {
                        sql.AppendLine(whereLine);
                    }
                    string idsArr = $"( { string.Join(",",filter.ChatIds) } )";
                    sql.AppendLine($"chatId in {idsArr}");
                    
                }
            }
            if (limit != -1)
            {
                sql.AppendLine(limitLine);
            }
            sql.AppendLine(";");
            return await DbHelper.Query<MessageModel>(sql.ToString(), new {FromDate = filter.DateOptions?.FromDate, TillDate = filter.DateOptions?.TillDate });
        }

        public async Task<IEnumerable<MessageModel>> OldestMessages()
        {
            string sql = """select * from (select  min("Date") as "Date",chatid  from message m group by (chatId) )as f;""";
            return await DbHelper.Query<MessageModel>(sql,null);
        }
    }
}

