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
        public Node<TKey> Head { get; init; }
        public override string ToString()
        {
            return Head.ToString();
        }
    }
    public class Node<Tkey>
    {
        public Tkey Key { get; init; }
        public HashSet<Node<Tkey>> Nodes { get; private set; }
        /// <summary>
        /// return true if node were contained
        /// </summary>
        /// <returns></returns>
        public bool RelateNodesBidirectionaly(Node<Tkey> node)
        {
            if (Nodes == null)
            {
                Nodes = new HashSet<Node<Tkey>>();
            }
            if (node.Nodes == null)
            {
                node.RelateNodesBidirectionaly(this);
            }
            bool state = this.Nodes.Add(node);
            node.Nodes.Add(this);
            return state;
        }
        public bool RelateNodesUnidirectionally(Node<Tkey> node)
        {
            if (Nodes == null)
            {
                Nodes = new HashSet<Node<Tkey>>();
            }
            bool state = this.Nodes.Add(node);
            return state;
        }
        public override string ToString()
        {
            return $"Key:{Key.ToString()}";
        }
    }
}
