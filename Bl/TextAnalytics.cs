using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
