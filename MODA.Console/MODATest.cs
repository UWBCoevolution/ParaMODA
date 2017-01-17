﻿using MODA.Impl;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;
using StdConsole = System.Console;
using MODA.Impl.Graphics;

namespace MODA.Console
{
    public class MODATest
    {
        internal static void Run(string[] args)
        {
            try
            {
                #region Process input parameters
                if (args == null || args.Length != 6)
                {
                    StdConsole.ForegroundColor = ConsoleColor.Red;
                    try
                    {
                        StdConsole.WriteLine(File.ReadAllText("Files/ReadMe.txt"));
                    }
                    catch
                    {
                        StdConsole.WriteLine("Error. Use the command:\nMODA.Console <graphFolder> <filename> <subGraphSize> <threshold> <getOnlyMappingCounts> <useModifiedGrochow>\nSee ReadMe.txt file for more details.");
                    }
                    StdConsole.ForegroundColor = ConsoleColor.White;
                    return;
                }
                string graphFolder = args[0]; // @"C:\SOMA\Drive\MyUW\Research\Kim\remodaalgorithmimplementation";
                string filename = args[1]; // "QueryGraph.txt"; // "Ecoli20141001CR_idx.txt";
                var inputGraphFile = Path.Combine(graphFolder, filename);
                int subGraphSize;
                if (!int.TryParse(args[2], out subGraphSize) || subGraphSize <= 2)
                {
                    throw new ArgumentException("Invalid input for <subGraphSize> argument (arg[2]). Value should be an integer greater than 2.");
                }
                int threshold;
                if (!int.TryParse(args[3], out threshold))
                {
                    throw new ArgumentException("Invalid input for <threshold> argument (arg[3])");
                }
                string getOnlyMappingCounts = args[4];
                if (getOnlyMappingCounts == "y" || getOnlyMappingCounts == "Y")
                {
                    ModaAlgorithms.GetOnlyMappingCounts = true;
                }
                string useModifiedGrochow = args[5];
                if (useModifiedGrochow == "y" || useModifiedGrochow == "Y")
                {
                    ModaAlgorithms.UseModifiedGrochow = true;
                }
                var sb = new StringBuilder("Processing Graph...");
                sb.AppendFormat("Network File: {0}\nSub-graph Size: {1}\n", inputGraphFile, subGraphSize);
                sb.AppendLine("==============================================================\n");
                StdConsole.WriteLine(sb);
                sb.Clear();

                var inputGraph = GraphProcessor.LoadGraph(inputGraphFile);
                string queryGraphFile = null;
                QueryGraph queryGraph = null;
                if (subGraphSize > 5)
                {
                    StdConsole.WriteLine("You need a query graph to proceed. To supply the query graph file, type 'y' and press Enter.");
                }
                else
                {
                    StdConsole.WriteLine("Do you have a particular size {0} query graph in mind? Y/N", subGraphSize);
                }
                string resp = StdConsole.ReadLine();
                if (resp == "y" || resp == "Y" || subGraphSize > 5)
                {
                    while (true)
                    {
                        StdConsole.WriteLine("Enter the (relative or absolute) path to the query graph file");
                        queryGraphFile = StdConsole.ReadLine();
                        queryGraph = GraphProcessor.LoadGraph(queryGraphFile, true) as QueryGraph;
                        if (queryGraph.VertexCount != subGraphSize)
                        {
                            StdConsole.WriteLine("The specified subgraph size does not match with the query graph size. \nDo you want to use the size of the specified query graph instead? Y/N");
                            resp = StdConsole.ReadLine();
                            if (resp == "y" || resp == "Y")
                            {
                                subGraphSize = queryGraph.VertexCount;
                                break;
                            }
                            // else contiue;
                        }
                        else // we're good. So,
                        {
                            break;
                        }
                    }
                }

                if (subGraphSize >= inputGraph.VertexCount)
                {
                    throw new NotSupportedException("The specified subgraaph size is too large.");
                }
                StdConsole.WriteLine("Input Graph (G): Nodes - {0}; Edges: {1}\n", inputGraph.VertexCount, inputGraph.EdgeCount);
                if (queryGraph != null)
                {
                    StdConsole.WriteLine("Query Graph (H): Nodes - {0}; Edges: {1}\n", queryGraph.VertexCount, queryGraph.EdgeCount);
                }

                StdConsole.WriteLine("Do you want to generate an image of the input (and query) graph(s)? Y/N\nIf Y, you'll need to provide the path to dot.exe program on your machine");
                resp = StdConsole.ReadLine();
                if (resp == "y" || resp == "Y")
                {
                    StdConsole.WriteLine("Enter the path of the dot.exe program on your machine:");
                    resp = StdConsole.ReadLine(); //the dot program's filename, including the path
                    Visualizer.Visualize(inputGraph, resp, inputGraphFile + ".dot");
                    if (queryGraph != null) // => queryGraphFile has a value
                    {
                        Visualizer.Visualize(queryGraph, resp, queryGraphFile + ".dot");
                    }
                }
                StdConsole.ForegroundColor = ConsoleColor.Green;

                if (queryGraph == null)
                {
                    ModaAlgorithms.BuildTree(subGraphSize);
                }
                ModaAlgorithms.Threshold = threshold;
                ModaAlgorithms.QueryGraph = queryGraph;

                #endregion

                var sw = Stopwatch.StartNew();

                var frequentSubgraphs = ModaAlgorithms.Algorithm1(inputGraph, subGraphSize);

                sw.Stop();

                #region Process output
                long totalMappings = 0;
                sb.Append("\nCompleted. Result Summary\n");
                sb.AppendLine("-------------------------------------------\n");
                if (ModaAlgorithms.GetOnlyMappingCounts)
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        int count = (int)qGraph.Value;
                        sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\t Is Frequent Subgraph? {2}\n", qGraph.Key.AsString(), count, qGraph.Key.IsFrequentSubgraph);
                        totalMappings += count;
                    }
                }
                else
                {
                    foreach (var qGraph in frequentSubgraphs)
                    {
                        int count = ((System.Collections.Generic.List<Mapping>)qGraph.Value).Count;
                        sb.AppendFormat("\tSub-graph: {0}\t Mappings: {1}\t Is Frequent Subgraph? {2}\n", qGraph.Key.AsString(), count, qGraph.Key.IsFrequentSubgraph);
                        foreach (var mapping in ((System.Collections.Generic.List<Mapping>)qGraph.Value))
                        {
                            sb.AppendFormat("\t\t{0}", mapping);
                        }
                        totalMappings += count;
                    }
                }
                sb.AppendFormat("\nTime Taken: {0} ({1}ms)\nNetwork: Nodes - {2}; Edges: {3};\nTotal Mappings found: {4}\nSubgraph Size: {5}\n", sw.Elapsed, sw.ElapsedMilliseconds.ToString("N"), inputGraph.VertexCount, inputGraph.EdgeCount, totalMappings, subGraphSize);
                sb.AppendLine("-------------------------------------------\n");
                inputGraph = null;
                frequentSubgraphs = null;
                StdConsole.ForegroundColor = ConsoleColor.Blue;
                StdConsole.WriteLine(sb);

                try
                {
                    File.WriteAllText(inputGraphFile + ".OUT", sb.ToString());
                }
                catch { }

                #endregion

                StdConsole.ForegroundColor = ConsoleColor.White;
#if !DEBUG
                StdConsole.WriteLine("Done! Press any key to exit.");
#endif
            }
            catch (Exception ex)
            {
                StdConsole.ForegroundColor = ConsoleColor.Red;
                StdConsole.WriteLine(ex);
                StdConsole.ForegroundColor = ConsoleColor.White;
            }
#if DEBUG
            StdConsole.ReadKey();
#endif
        }
    }
}
