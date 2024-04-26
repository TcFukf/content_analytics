using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.structures
{
    public class WeightedGraph<Tvalue> : Graph<Tvalue>
    {
        List<double> Weights { get; set; }
        public WeightedGraph(Tvalue val,Graph<Tvalue>[] graphs, double[] weights) : base(val, graphs)
        {
            if (weights.Length != graphs.Length)
            {
                throw new ArgumentException("weights.Length != graphs.Length");
            }
            Weights = weights.ToList();
            
        }
    }
}
