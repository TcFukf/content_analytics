

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


static void CreateFreqDictsFromFiles(string currentDir, string bookPath, string ouotr, string[] files, Queue<Thread> threads, int threadCount)
{
    foreach (var file in files)
    {
        Console.WriteLine(file);
        while (threads.Count() >= threadCount)
        {
            Console.WriteLine("wat");
            Thread.Sleep(3 * 60_000);
        }
        var th = new Thread(() =>
        {
            Console.WriteLine($"{file} started, {threads.Count}");
            string fileName = Path.GetFileNameWithoutExtension(file);
            ParsBigFile.CreateFreqDictionarieFromFile(file, currentDir + bookPath + "\\" + ouotr + "\\" + fileName);
            Console.WriteLine($"{file} ended, {threads.Count}");
            lock (threads)
            {
                if (threads.Count > 0)
                {
                    threads.Dequeue();
                }
            }
        });
        threads.Enqueue(th);
        th.Start();
    }
    while (threads.Count > 0)
    {
        Console.WriteLine("running");
        Thread.Sleep(60_000);
    }
}

static void ShowSimilarities(TextAnalyzer textAnalyzer, MessageModel[] messages, MessageModel post)
{
    var results = textAnalyzer.TextSimilarity(post.Text, 100, messages.Select(mg=>mg.Text).ToArray()).ToArray();
    ((long, long), double)[] res = new ((long, long), double)[results.Length];
    for (int i = 0; i < results.Length; i++)
    {
        res[i] = ((messages[i].MessageId, messages[i].ChatId ), results[i]);
    }
    LogTools.PrintIE(res.OrderBy(t => t.Item2));
}
