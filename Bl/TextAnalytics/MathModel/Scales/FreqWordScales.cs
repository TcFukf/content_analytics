using social_analytics.Bl.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return frequencyDict.GetKeyFrequency(word);
        }

        public double CalcWeight(string word)
        {
            if( frequencyDict.GetKeyCount(word) == 0)
            {
                //Console.WriteLine($"defScales* NOT IN SKYE :{word}");
                return 0;
            }
            return 1 / (frequencyDict.GetKeyFrequency(word)) * Math.Pow(word.Length, 1d);
        }

        public string GetScalse(string word)
        {
            return frequencyDict.WordTranformator.TransformWord(word);
        }
    }
}
