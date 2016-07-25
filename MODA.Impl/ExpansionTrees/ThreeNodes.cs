﻿using MODA.Impl.Graphics;
using QuickGraph;
using System.Collections.Generic;
using System.IO;

namespace MODA.Impl
{
    public static class ThreeNodes
    {
        static string graphFolder = @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expansionTree"></param>
        /// <returns>The root node</returns>
        public static ExpansionTreeNode<Edge<int>> BuildThreeNodesTree(this AdjacencyGraph<ExpansionTreeNode<Edge<int>>, Edge<ExpansionTreeNode<Edge<int>>>> expansionTree)
        {
            //Level 0 - Root Node
            var rootNode = new ExpansionTreeNode<Edge<int>>();

            //Level 1
            var qGraphL1_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var nodeL1_1 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 1,
                QueryGraph = qGraphL1_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(rootNode, nodeL1_1));

            string filename = "QueryTree3.txt";
            var dotFileFulllName = Path.Combine(graphFolder, filename);
            Visualizer.Visualize(qGraphL1_1, dotFileFulllName + ".dot");

            //Level 2
            var qGraphL2_1 = new List<Edge<int>>()
            {
                new Edge<int>(0,1),
                new Edge<int>(1,2),
                new Edge<int>(0,2), //New Add;
            }
            .ToUndirectedGraph<int, Edge<int>>(false);
            var nodeL2_1 = new ExpansionTreeNode<Edge<int>>
            {
                Level = 2,
                QueryGraph = qGraphL2_1,
            };

            expansionTree.AddVerticesAndEdge(new Edge<ExpansionTreeNode<Edge<int>>>(nodeL1_1, nodeL2_1));

            return rootNode;
        }
    }
}
