﻿//This is the one that has gone bad
using QuickGraph;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
        private static List<Mapping> Algorithm2_Modified(QueryGraph queryGraph, UndirectedGraph<int> inputGraph, int numberOfSamples, bool getInducedMappingsOnly)
        {
            if (numberOfSamples <= 0) numberOfSamples = inputGraph.VertexCount / 3;
            
            var theMappings = new Dictionary<IList<int>, Mapping>(MappingNodesComparer);
            var inputGraphDegSeq = inputGraph.GetNodesSortedByDegree(numberOfSamples);

            var threadName = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("Thread {0}:\tCalling Algo 2-Modified:\n", threadName);

            var h = queryGraph.Vertices.ElementAt(0);
            var f = new Dictionary<int, int>(1);
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                if (Utils.CanSupport(queryGraph, h, inputGraph, g))
                {
                    #region Can Support
                    //Remember: f(h) = g, so h is Domain and g is Range
                    f[h] = g;
                    var mappings = Utils.IsomorphicExtension(f, queryGraph, inputGraph, getInducedMappingsOnly);
                    if (mappings.Count > 0)
                    {
                        foreach (var item in mappings)
                        {
                            //Recall: f(h) = g
                            theMappings[item.Key] = item.Value;
                        }
                        mappings = null;
                    }
                    #endregion
                }
            }

            var toReturn = new List<Mapping>(theMappings.Values);
            theMappings = null;
            inputGraphDegSeq = null;
            
            Console.WriteLine("\nThread {0}:\tAlgorithm 2: All iteration tasks completed. Number of mappings found: {1}.\n", threadName, toReturn.Count);
            return toReturn;
        }

    }
}
