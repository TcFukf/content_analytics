

using social_analytics;
using social_analytics.Bl.BotServer;
using social_analytics.Bl.Filter;
using social_analytics.Bl.Messages;
using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics;
using social_analytics.Bl.TextAnalytics.MathModel;
using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using social_analytics.Bl.TextAnalytics.MathModel.WordTransformers;
using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer;
using social_analytics.Bl.TextAnalytics.TextClassifier;
using social_analytics.Bl.TextAnalytics.TextFiles;
using social_analytics.DAL;
using social_analytics.Helpers;
using System.Data.Common;
using Telegram.Td.Api;
using TelegramWrapper.Models;
using TelegramWrapper.TelegramParser;
using TelegramWrapper.Wrapper;
using TelegramWrapper.Wrapper.Bl;



string currentDir = Directory.GetCurrentDirectory();
string bigFilePath = "\\books\\wiki\\";
string freqDictPath = "\\freqDict\\";
var contextManager = new ContextManager();
IClientWrapper tg = new TelegramClient(
                                       TelegramWrapper.Helpers.ConfigHelper.ApiId,
                                       TelegramWrapper.Helpers.ConfigHelper.ApiHash,
                                       contextManager
                                       );

MessagesService msgService = new MessagesService(new Parser(tg), new MessageDAL());

FrequencySkye freqSkye = new FrequencySkye(new FrequencyDictionary<string>(),new PorterTransformator());
freqSkye.LoadWordsFromJsonFile("./"+freqDictPath+ "allwiki.json");
ITagScales scales = new FreqWordScales(freqSkye);
TextAnalyzer textAnalyzer = new(scales);



tg.InitAndWaitAuthorize();

List<IChatContext> dataProviderChats = new();
List<IChatContext> sendingChats = new();
Console.WriteLine("INDEXED CHATS:");
foreach (var chat in tg.GetNotPrivateChats().Result)
{
    Message last = chat.LastMessage;
    if (last?.CanBeDeletedForAllUsers == true)
    {
        sendingChats.Add(tg.GetOrCreateChatContextWith(chat.Id));
    }
    else
    {
        Console.WriteLine(chat.Title);
        dataProviderChats.Add( tg.GetOrCreateChatContextWith(chat.Id) );
    }
}
MessageSearchOptions filter = new()
{
    DateOptions = new()
    {

        TillDate = DateTime.UtcNow.Date,
        FromDate = DateTime.UtcNow.AddDays(-1).Date
    }
};
NewsPoster contentPoster = new NewsPoster(textAnalyzer,tg,sendingChats.Select(x=>x.chatId).ToList());
foreach (var ctx in dataProviderChats)
{
    contentPoster.ListenContext(ctx);
}

ClientServer tgServer = new ClientServer(msgService,textAnalyzer,tg);
Console.WriteLine("START LISTENING");
Task.Run(()=>tgServer.ListenRequests() ).Wait();

