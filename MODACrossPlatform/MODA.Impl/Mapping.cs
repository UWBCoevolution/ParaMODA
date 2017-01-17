﻿using QuickGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace MODA.Impl
{
    
    public class Mapping
    {
        public Mapping(Dictionary<string, string> function)
        {
            Function = function;
        }
        
        /// <summary>
        /// This represents the [f(h) = g] relation. Meaning key is h and value is g.
        /// </summary>
        public Dictionary<string, string> Function { get; private set; }
        
        ///// <summary>
        ///// The subgraph (with all edges) in the input graph G that fit the query graph (---Function.Values)
        ///// </summary>
        //public UndirectedGraph<string, Edge<string>> InputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>();
        
        /// <summary>
        /// The subgraph (with mapped edges) in the input graph G that fit the query graph (---Function.Keys)
        /// </summary>
        public UndirectedGraph<string, Edge<string>> MapOnInputSubGraph { get; set; } = new UndirectedGraph<string, Edge<string>>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            var functionSorted = new SortedDictionary<string, string>(Function);
            sb.Append("[");
            foreach (var item in functionSorted)
            {
                sb.AppendFormat("{0}-", item.Key);
            }
            //sb.AppendFormat("<{0}>] => [", this.MapOnInputSubGraph.AsString());
            sb.Append("] => [");
            foreach (var item in functionSorted)
            {
                sb.AppendFormat("{0}-", item.Value);
            }
            //sb.AppendFormat("] Exact map: <{0}>\n", this.MapOnInputSubGraph.AsString());
            sb.Append("]\n");
            //sb.AppendLine("]");
            return sb.ToString();
        }
    }
}
