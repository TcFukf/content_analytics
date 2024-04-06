using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelegramWrapper.Models;

namespace social_analytics.Bl
{
    public class TextAnalytics : ITextAnalytics
    {
        public Task GetMostFrequentlyNouns()
        {
            throw new NotImplementedException();
        }

        public Task GetNouns(string text)
        {
            var a = new TextEntity("word",0);
            throw new NotImplementedException();
        }
        static public Dictionary<string, int> GetStat(params string[] inputs)
        {
            Dictionary<string, int> stat = new();
            foreach (var input in inputs)
            {
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }
                var matched = Regex.Matches(input, @"[\w]+");
                foreach (Match match in matched)
                {
                    string value = match.Value.ToLower();
                    if (stat.ContainsKey(value) )
                    {
                        stat[value] += 1;
                    }
                    else
                    {
                        stat[value] = 1;
                    }
                }
            }
            return stat;
        }
    }
}
