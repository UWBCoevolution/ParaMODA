﻿using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MODA.Impl
{
    public partial class ModaAlgorithms
    {
        /// <summary>
        /// Mapping module; aka FindSubgraphInstances in Grochow & Kellis
        /// </summary>
        /// <param name="queryGraph">H</param>
        /// <param name="inputGraphClone">G</param>
        /// <param name="numberOfSamples">To be decided. If not set, we use the <paramref name="inputGraphClone"/> size / 3</param>
        private static List<Mapping> Algorithm2(QueryGraph queryGraph, UndirectedGraph<string, Edge<string>> inputGraphClone, int numberOfSamples = -1)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            if (numberOfSamples <= 0) numberOfSamples = inputGraphClone.VertexCount / 3; // VertexCountDividend;

            // Do we need this clone? Can't we just remove the node directly from the graph? 
            // We do need it.

            //var comparer = new MappingNodesComparer();
            //InputSubgraphs = new Dictionary<string[], UndirectedGraph<string, Edge<string>>>(comparer);
            //MostConstrainedNeighbours = new Dictionary<string[], string>(comparer);
            H_NodeNeighbours = new Dictionary<string, IList<string>>();
            var theMappings = new Dictionary<string, List<Mapping>>();
            var inputGraphDegSeq = inputGraphClone.GetDegreeSequence(numberOfSamples);
            var queryGraphVertices = queryGraph.Vertices.ToArray();
            var subgraphSize = queryGraphVertices.Length;
            Console.WriteLine("Calling Algo 2:\n");
            for (int i = 0; i < inputGraphDegSeq.Count; i++)
            {
                var g = inputGraphDegSeq[i];
                //NeighboursOfRange = new Dictionary<string[], List<string>>(comparer);
                G_NodeNeighbours = new Dictionary<string, IList<string>>();
                for (int j = 0; j < subgraphSize; j++)
                {
                    var h = queryGraphVertices[j];
                    if (CanSupport(queryGraph, h, inputGraphClone, g))
                    {
                        #region Can Support
                        //var sw = System.Diagnostics.Stopwatch.StartNew();
                        //Remember: f(h) = g, so h is Domain and g is Range
                        var mappings = IsomorphicExtension(new Dictionary<string, string>(1) { { h, g } }, queryGraph, inputGraphClone);

                        //sw.Stop();
                        //Console.WriteLine("Time to do IsomorphicExtension: {0}\n", sw.Elapsed.ToString());
                        //Console.Write(".");
                        if (mappings != null && mappings.Count > 0)
                        {
                            //sw.Restart();

                            for (int k = 0; k < mappings.Count; k++)
                            {
                                Mapping mapping = mappings[k];
                                List<Mapping> mappingsToSearch; //Recall: f(h) = g
                                //var g_key = mapping.Function.ElementAt(subgraphSize - 1).Value;
                                var g_key = mapping.Function.Last().Value;

                                if (theMappings.TryGetValue(g_key, out mappingsToSearch))
                                {
                                    if (!mappingsToSearch.Exists(x => x.IsIsomorphicWith(mapping)))
                                    //if (mappingsToSearch.Find(x => x.IsIsomorphicWith(mapping)) == null)
                                    {
                                        theMappings[g_key].Add(mapping);
                                    }
                                }
                                else
                                {
                                    theMappings[g_key] = new List<Mapping> { mapping };
                                }
                                mappingsToSearch = null;
                            }

                            //sw.Stop();
                            //Console.WriteLine("Map: {0}.\tTime to set:\t{1:N}s.\th = {2}. g = {3}\n", mappings.Count, sw.Elapsed.ToString(), queryGraphVertices[j], inputGraphDegSeq[i]);
                            //sw = null;
                            mappings = null;
                        }
                        #endregion
                    }
                }

                //Remove g
                inputGraphClone.RemoveVertex(g);
                //NeighboursOfRange = null;
                G_NodeNeighbours = null;
            }

            var toReturn = new List<Mapping>();
            foreach (var mapping in theMappings)
            {
                toReturn.AddRange(mapping.Value);
            }
            //Console.WriteLine("\nAlgorithm 2: All iteration tasks completed. Number of mappings found: {0}.\n", toReturn.Count);
            timer.Stop();
            Console.WriteLine("Algorithm 2: All tasks completed. Number of mappings found: {0}.\nTotal time taken: {1}", toReturn.Count, timer.Elapsed.ToString());
            timer = null;
            theMappings = null;
            inputGraphDegSeq = null;
            queryGraphVertices = null;
            inputGraphClone = null;
            G_NodeNeighbours = null;
            H_NodeNeighbours = null;
            MostConstrainedNeighbours = null;
            NeighboursOfRange = null;
            return toReturn;
        }
    }
}
