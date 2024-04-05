using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl
{
    public struct TextEntity
    {
        public string Value { get; set; }
        public int Count { get; set; }
        public TextEntity(string value, int count)
        {
            throw new Exception("think u dont need it )) ");
            Value = value;
            Count = count;
        }
    }
}
