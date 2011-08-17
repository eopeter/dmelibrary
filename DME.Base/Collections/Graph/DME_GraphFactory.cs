using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DME_GraphFactory<T>
    {
        /// <summary>
        /// 创建一个简单的关系图
        /// </summary>
        /// <returns></returns>
        public static DME_Graph<int> CreateSimpleGraph()
        {
            DME_Graph<int> graph = new DME_Graph<int>();
            int len = 20;
            int i = 1;
            //图中有len个接点
            for (i = 0; i < len; i++)
            {
                graph.Nodes.Add(new DME_GraphNode<int>(i));
            }
            //随机创建40条边，可重复
            Random random = new Random();
            for (i = 0; i < 40; i++)
            {
                int n1 = random.Next(0, len);
                int n2 = random.Next(0, len);
                //如果随机到相同的点
                if (n1 == n2)
                    n2 = (n1 + 1) % len;
                graph.Nodes[n1].AssociatedNodes.Add(graph.Nodes[n2]);
                graph.Nodes[n2].AssociatedNodes.Add(graph.Nodes[n1]);
            }
            return graph;
        }
        /// <summary>
        /// 打印图
        /// 显示各接点的关系接点
        /// </summary>
        /// <param name="graph"></param>
        public static void PrintGraph(DME_Graph<T> graph)
        {
            foreach (DME_GraphNode<T> node in graph.Nodes)
            {
                Console.Write("Node {0} connected to: ", node.Value);
                foreach (DME_GraphNode<T> associatednode in node.AssociatedNodes)
                {
                    Console.Write("\t{0}", associatednode.Value);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// 深度优先遍历
        /// </summary>
        /// <param name="graph"></param>
        public static void DepthFirstTraversal(DME_Graph<T> graph)
        {
            Reset(graph);
            if (graph.Nodes.Count == 0)
            {
                return;
            }
            DME_GraphNode<T> node = GetNodeUnVisited(graph);
            while (node != null)
            {
                DepthFirstTraversal_Node(node);
                node = GetNodeUnVisited(graph);
            }
        }
        /// <summary>
        /// 广度优先遍历
        /// </summary>
        /// <param name="graph"></param>
        public static void BreadthFirstTraversal(DME_Graph<T> graph)
        {
            Reset(graph);
            if (graph.Nodes.Count == 0)
            {
                return;

            }
            DME_GraphNode<T> node = GetNodeUnVisited(graph);
            Queue<DME_GraphNode<T>> nodequeue = new Queue<DME_GraphNode<T>>();
            while (node != null)
            {
                BreadthFirstTraversal_Node(node, nodequeue);
                node = nodequeue.Dequeue();
                if (node == null)
                    node = GetNodeUnVisited(graph);
            }
        }
        #region private method
        /// <summary>
        /// 深度优先遍历接点
        /// 先访问该接点，然后递归其关系接点
        /// </summary>
        /// <param name="node"></param>
        private static void DepthFirstTraversal_Node(DME_GraphNode<T> node)
        {
            if (node.IsVisited || node == null)
            {
                return;
            }
            node.IsVisited = true;
            Console.WriteLine("Node {0} is visited!", node.Value.ToString());
            foreach (DME_GraphNode<T> associatednode in node.AssociatedNodes)
            {
                DepthFirstTraversal_Node(associatednode);
            }
        }
        /// <summary>
        /// 广度优先遍历接点
        /// 访问该接点(如果该接点未被访问)，然后将其关系接点(如果该接点未被访问)放入队列中
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodequeue"></param>
        private static void BreadthFirstTraversal_Node(DME_GraphNode<T> node, Queue<DME_GraphNode<T>> nodequeue)
        {
            if (node.IsVisited || node == null)
                return;
            node.IsVisited = true;
            Console.WriteLine("Node {0} is visited!", node.Value.ToString());
            foreach (DME_GraphNode<T> n in node.AssociatedNodes)
            {
                if (!n.IsVisited)
                {
                    nodequeue.Enqueue(n);
                }
            }
        }
        /// <summary>
        /// 获取图中一个未访问的接点
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private static DME_GraphNode<T> GetNodeUnVisited(DME_Graph<T> graph)
        {
            foreach (DME_GraphNode<T> node in graph.Nodes)
                if (!node.IsVisited)
                    return node;
            return null;
        }
        /// <summary>
        /// 重新设置图中各接点的访问状态为false
        /// </summary>
        /// <param name="graph"></param>
        private static void Reset(DME_Graph<T> graph)
        {
            foreach (DME_GraphNode<T> node in graph.Nodes)
            {
                node.IsVisited = false;
            }
        }
        #endregion
    }
}
