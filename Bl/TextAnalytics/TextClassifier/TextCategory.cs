using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.TextAnalytics.TextClassifier
{
    public class TextCategory<TItems>
    {
        public WordTagsVector HeadVector { get; set; }
        public List<TItems> Itimes   { get; set; }
        public override int GetHashCode()
        {
            return HeadVector.GetHashCode();
        }
    }
}
