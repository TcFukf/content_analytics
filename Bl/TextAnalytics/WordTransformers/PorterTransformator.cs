using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics.WordTransformers
{
    public class PorterTransformator : IWordTransforamtor
    {
        public string TransformWord(string word)
        {
            return PorterStemming.TransformWord(word);
        }
    }
}
