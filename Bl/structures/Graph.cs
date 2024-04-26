using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.structures
{
    public class Graph<Tvalue>
    {
        public Tvalue Value { get; set; }
        public List<Graph<Tvalue>> Neighbours { get; set; }
        public Graph(Tvalue val, params Graph<Tvalue>[] graphs )
        {
            Value = val;
            Neighbours = graphs.ToList();
        }
        public Graph()
        {
                
        }
    }

}
