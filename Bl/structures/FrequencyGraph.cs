using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.structures
{
    public class FrequencyGraph<T>
    {
        public T Value { get; set; }
        public IFrequencyDictionary<T> Neightboors { get; set; }
        public FrequencyGraph(T value, IFrequencyDictionary<T> neight)
        {
            Value = value;
            Neightboors = neight;
        }
        
        public override string ToString()
        {
            return "val:" + Value?.ToString() + "  Count :   " + Neightboors.Count();
        }
    }
}
