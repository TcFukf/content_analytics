
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
var messages = new List<MessageModel>();
foreach (var id in chatIds)
{
     messages.AddRange( pars.GetMessages(id,10).Result.Select(m=>ModelConverter.FromTelegramMessageToWrapped(m)) );
    
}
var res = GetStat(messages);
Print(res);

static Dictionary<string,int> GetStat(IEnumerable<MessageModel> messages)
{
    Dictionary<string, int> stat = new();
    foreach (var msg in messages)
    {
        if (string.IsNullOrEmpty(msg.Text))
        {
            continue;
        }
        var matched = Regex.Matches(msg.Text,@"\w+");
        foreach (Match match in matched)
        {
            if (stat.ContainsKey(match.Value))
            {
                stat[match.Value] += 1;
            }
            else
            {
                stat[match.Value] = 1;
            }
        }
    }
    return stat;
}
static void Print<T>(IEnumerable<T> arr)
{
    Console.WriteLine(string.Join(", ", arr));
}