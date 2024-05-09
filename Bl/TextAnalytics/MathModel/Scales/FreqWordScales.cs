using social_analytics.Bl.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Td.Api;

namespace social_analytics.Bl.TextAnalytics.MathModel.Scales
{
    public class FreqWordScales : ITagScales
    {
        private readonly FrequencySkye frequencyDict;
        public FreqWordScales(FrequencySkye weighter)
        {
            frequencyDict = weighter;
        }
        public double CalcOrder(string word)
        {
            return CalcWeight(word);
        }

        public double CalcWeight(string word)
        {
            if( frequencyDict.GetKeyCount(word) == 0)
            {
                return 0;
            }
            // 1/keyFreq too big
            double weight = (1 / frequencyDict.GetKeyFrequency(word) ) * Math.Pow(word.Length, 1d);
            return weight/100_000_000;
        }

        public string GetScalse(string word)
        {
            return frequencyDict.WordTranformator.TransformWord(word);
        }
    }
}
