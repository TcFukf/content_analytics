
using social_analytics;
using social_analytics.Bl;
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
tg.InitAndWaitAuthorize();
var chatIds = (tg.GetChats().Result);
var pars = new Parser(tg);
var msgs = Parser.ReadMessagesFromLocal("F:\\vsLocations\\Location\\TelegramWrapper\\root");
int count = 0;
foreach (var el in msgs)
{
    count += el.Item2.Count();
}
var test = new TrySomething();
foreach (var item in msgs.GetRange(2,2))
{
    string[] word = TextAnalytics.GetEntities( string.Join(" ",item.messages.Select(m=>m.Text)) ).Select(m=>m.Value).ToArray();
    Console.WriteLine( string.Join(Environment.NewLine, test.FindEntityGroups(4,word) )  );
}


static void MergeStat(Dictionary<string, int> outStat, Dictionary<string, int> addStat)
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
static void Print<T>(IEnumerable<T> arr)
{
    Console.WriteLine(string.Join(", ", arr));
}