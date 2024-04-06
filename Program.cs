
using social_analytics;
using social_analytics.Bl;
using System.Diagnostics.SymbolStore;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Td.Api;
using TelegramWrapper.Helpers;
using TelegramWrapper.Models;
using TelegramWrapper.TelegramParser;
using TelegramWrapper.Wrapper;
using TelegramWrapper.Wrapper.Bl;
using TelegramWrapper.Wrapper.Bl.Handlers;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
//zlib1.dll
//libssl-3-x64.dll
//libcrypto-3-x64.dll
//Assembly.LoadFrom("./libs/Telegram.Td.dll");
//Assembly.LoadFrom("./libs/libssl-3-x64.dll");
//Assembly.LoadFrom("./libs/libcrypto-3-x64.dll");
//Assembly.LoadFrom("./libs/zlib1.dll");



IClientWrapper tg = new TelegramClient(TelegramWrapper.Helpers.ConfigHelper.ApiId,
                                       TelegramWrapper.Helpers.ConfigHelper.ApiHash,
                                       new ContextManager());
//tg.InitAndWaitAuthorize();
//var chatIds = (tg.GetChats().Result);
//var pars = new Parser(tg);
var chats = Parser.ReadMessagesFromLocal("F:\\vsLocations\\Location\\TelegramWrapper\\root");
int count = 0;
foreach (var chat in chats)
{
    count += chat.messages.Count();
}
string[] words = TextAnalytics.GetStringEntities(TrySomething.text).ToArray();
var gp = TextAnalytics.GetGpaphs(2, words);
var test = new TrySomething();

Dictionary<string, FrequencyDictionary<string>> d = new();
foreach (var graph in gp)
{
    if (d.ContainsKey(graph.Value))
    {
        d[graph.Value].Plus(graph.Neightboors as FrequencyDictionary<string>);
    }
    else
    {
        d[graph.Value] = graph.Neightboors as FrequencyDictionary<string>;
    }
}


Console.WriteLine(TrySomething.text);
Console.WriteLine(string.Join(Environment.NewLine, test.FindEntityGroups(4, TextAnalytics.GetStringEntities(TrySomething.text).ToArray())));



static void MergeDictionariesInt<Tkey>(Dictionary<Tkey, int> outStat, Dictionary<Tkey, int> addStat)
{
    foreach (var key in addStat.Keys)
    {
        if (outStat.ContainsKey(key))
        {
            outStat[key] += addStat[key];
        }
        else
        {
            outStat[key] = addStat[key];
        }
    }
}
static void MergeDictionaries<Tkey,TValue>(Dictionary<Tkey, IHavePlustOperation<TValue>> outStat, Dictionary<Tkey, IHavePlustOperation<TValue>> addStat ) where TValue:IHavePlustOperation<TValue>
{
    foreach (var key in addStat.Keys)
    {
        if (outStat.ContainsKey(key))
        {
            outStat[key] = outStat[key].Plus(addStat[key].Value() );
        }
        else
        {
            outStat[key] = addStat[key];
        }
    }
}
static void Print<T>(IEnumerable<T> arr)
{
    Console.WriteLine(string.Join(", ", arr));
}