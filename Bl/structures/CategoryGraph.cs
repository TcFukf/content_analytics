using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace social_analytics.Bl.structures
{
    public class CategoryGraph<TKey>
    {
        public HashSet<TKey> Nodes { get; init; }
    }
    public class Node<Tkey>
    {
        public Tkey IdKey { get; init; }
        public List<Node<Tkey>> Nodes { get; init; }
    }
}
