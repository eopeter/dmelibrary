using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    public class DME_Graph<T>
    {
        /// <summary>
        /// 图包含接点      
        /// </summary>        
        public List<DME_GraphNode<T>> Nodes;
        public DME_Graph()
        {
            Nodes = new List<DME_GraphNode<T>>();
        }
    }
}
