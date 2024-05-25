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
        public CategoryNode<TKey> Head { get; init; }
        public override string ToString()
        {
            return Head.ToString();
        }
    }
    public class CategoryNode<Tkey>
    {
        public Tkey Key { get; init; }
        public HashSet<CategoryNode<Tkey>> Nodes { get; private set; }
        /// <summary>
        /// return true if node were contained
        /// </summary>
        /// <returns></returns>
        public bool RelateNodesBidirectionaly(CategoryNode<Tkey> node)
        {
            if (this.Nodes == null)
            {
                Nodes = new HashSet<CategoryNode<Tkey>>();
            }
            if (node.Nodes == null)
            {
                node.RelateNodesBidirectionaly(this);
            }
            bool state = this.Nodes.Add(node);
            node.Nodes.Add(this);
            return state;
        }
        public bool RelateNodesUnidirectionally(CategoryNode<Tkey> node)
        {
            if (Nodes == null)
            {
                Nodes = new HashSet<CategoryNode<Tkey>>();
            }
            return this.Nodes.Add(node);
        }
        public override string ToString()
        {
            return $"Key:{Key.ToString()}";
        }
    }
}
