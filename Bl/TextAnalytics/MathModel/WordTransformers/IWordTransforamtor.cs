using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics.MathModel.WordTransformers
{
    public interface IWordTransforamtor
    {
        string TransformWord(string word);
    }
}
