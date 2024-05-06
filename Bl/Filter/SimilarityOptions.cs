using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.Filter
{
    public class SimilarityOptions
    {
        public string Language { get; set; } = "russian";
        public string FieldName { get; set; } = "Text";
        public string[] SimilarityWords { get; set; }


    }
}
