using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.Filter
{
    public class DateOptions
    {
        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        public DateOptions()
        {
            if (TillDate == null)
            {
                TillDate = DateTime.UtcNow;
            }
        }
    }
}
