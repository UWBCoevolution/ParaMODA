﻿//This is the one that has gone bad
using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module (aka FindSubgraphInstances in Grochow & Kellis) modified
        /// The modification:
        ///     Instead of traversing all nodes in the query graph (H) for each node in the input graph (G),
        ///     we simply use just one node h in H to traverse G. This makes it much easier to parallelize 
        ///     unlike the original algorithm, and eliminate the need for removing visited g from G.
        ///     
        ///     Testing will show whether this improves, worsens or makes no difference in performance.
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraph">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraph"/> size / 3</param>
        private static List<Mapping> Algorithm2_Modified(QueryGraph queryGraph, UndirectedGraph<int, Edge<int>> inputGraph, int numberOfSamples = -1)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount / 3;

            H_NodeNeighbours = new Dictionary<int, HashSet<int>>();
            G_NodeNeighbours = new Dictionary<int, HashSet<int>>();
            var comparer = new MappingNodesComparer();
            var theMappings = new Dictionary<IList<int>, Mapping>(comparer);
            var inputGraphDegSeq = inputGraph.GetNodesSortedByDegree(numberOfSamples);

            Console.WriteLine("Calling Algo 2-Modified: Number of Iterations: {0}.\n", numberOfSamples);

            var h = queryGraph.Vertices.ElementAt(0);
            var f = new Dictionary<int, int>(1);
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                if (CanSupport(queryGraph, h, inputGraph, g))
                {
                    #region Can Support
                    //Remember: f(h) = g, so h is Domain and g is Range
                    f[h] = g;
                    var mappings = IsomorphicExtension(f, queryGraph, inputGraph, comparer);
                    if (mappings.Count > 0)
                    {
                        foreach (var item in mappings)
                        {
                            //Recall: f(h) = g
                            theMappings[item.Key] = item.Value;
                        }
                        mappings.Clear();
                    }
                    #endregion
                }
            }

            var toReturn = new List<Mapping>(theMappings.Values);
            theMappings.Clear();
            theMappings = null;
            inputGraphDegSeq.Clear();
            inputGraphDegSeq = null;
            H_NodeNeighbours.Clear();
            H_NodeNeighbours = null;
            G_NodeNeighbours.Clear();
            G_NodeNeighbours = null;
            
            Console.WriteLine("\nAlgorithm 2: All iteration tasks completed. Number of mappings found: {0}.\n", toReturn.Count);
            return toReturn;
        }

    }
}
