using DeepMorphy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics.MathModel.Scales
{
    public class MorphTagScales : ITagScales
    {
        private readonly FrequencySkye frequencySkye;
        private readonly MorphAnalyzer morphAnalyzer;
        public MorphTagScales(FrequencySkye freqDict,MorphAnalyzer morph)
        {
            this.frequencySkye = freqDict;
            this.morphAnalyzer = morph;
        }
        public double CalcOrder(string word)
        {
            return frequencySkye.GetKeyFrequency(word);
        }

        public double CalcWeight(string word)
        {
            var gramm = morphAnalyzer.Parse(word).ToList().First();
            if (frequencySkye.GetKeyCount(word) == 0)
            {
                return 0;
            }
            if (gramm?.BestTag?.GramsDic["чр"] == "сущ")
            {
                return 1 /( frequencySkye.GetKeyFrequency(word)) * Math.Pow(word.Length, 1d) * 100;
            }
            return 1 / (frequencySkye.GetKeyFrequency(word)) * Math.Pow(word.Length, 1d) ;
        }

        public string GetScalse(string word)
        {
            return frequencySkye.WordTranformator.TransformWord(word);
        }
    }
}
