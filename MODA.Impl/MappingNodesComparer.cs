﻿using System.Collections.Generic;
using System.Linq;

namespace MODA.Impl
{
    /// <summary>
    /// This compares two <see cref="Mapping"/> nodes are equal. Used in dictionary where these nodes form the key.
    /// The node set of interest is that of the mapping images on G; i.e. the set that form the nodes in the Mapping's InducedSubGraph.
    /// The parameters to be compared are guaranteed to be distinct and of the same length
    /// </summary>
    internal sealed class MappingNodesComparer : EqualityComparer<string[]>
    {
        public override bool Equals(string[] x, string[] y)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (!y.Contains(x[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode(string[] obj)
        {
            int hash = 0;
            for (int i = obj.Length - 1; i > -1; i--)
            {
                hash += obj[i].GetHashCode();
            }
            return hash;
        }
    }
}
