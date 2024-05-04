using DeepMorphy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics.WordTransformers
{
    public class MorphTransformer : IWordTransforamtor
    {
        private readonly MorphAnalyzer morphAnalyzer;
        public MorphTransformer(MorphAnalyzer morphAnalyzer)
        {
            this.morphAnalyzer = morphAnalyzer;
        }
        public string TransformWord(string word)
        {
            return morphAnalyzer.Parse(word).First().BestTag.Lemma;
        }
    }
}
