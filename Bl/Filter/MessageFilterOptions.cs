using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.Filter
{
    public class MessageFilterOptions
    {
        /// <summary>
        /// included bound
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        /// till day not included
        /// </summary>
        public DateTime? TillDate { get; set; }


    }
}
