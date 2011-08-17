using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    public class DME_GraphNode<T>
    {
        /// <summary>
	    /// 接点值
	    /// </summary>
	    public T Value;
	    /// <summary>
	    /// 是否被访问过
	    /// </summary>
	    public bool IsVisited;
	    /// <summary>
	    /// 关系接点
	    /// </summary>
	    public List<DME_GraphNode<T>> AssociatedNodes;
        public DME_GraphNode(T value)
	    {
		    Value = value;
		    IsVisited = false;
            AssociatedNodes = new List<DME_GraphNode<T>>();
	    }
    }
}
