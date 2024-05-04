using social_analytics.Bl.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics
{
    public class WordTagScales : ITagScales
    {
        private readonly WordSkye frequencyDict;
        public WordTagScales(WordSkye weighter)
        {
            frequencyDict = weighter;
        }
        public double CalcOrder(string word)
        {
            return frequencyDict.GetKeyFrequency(word);
        }

        public double CalcWeight(string word)
        {
            if (frequencyDict.GetKeyCount(word) == 0)
            {
                return 0;
            }
            return (1/frequencyDict.GetKeyFrequency(word) * word.Length );
        }
    }
}
