using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.Filter
{
    public class MessageSearchOptions
    {
        /// <summary>
        /// included bound
        /// </summary>
        public DateOptions DateOptions { get; set; }
        public SimilarityOptions SimilarityOptions { get; set; }

        public MessageSearchOptions()
        {
        }
        /// <summary>
        /// till day not included
        /// </summary>


    }
}
