# ParaMODA

This work improves a motif-centric tool which will enable researchers find subgraph instances in the input network for only the subgraphs of interest – called query graphs – as opposed to finding instances for all possible (non-isomorphic) k-graphs. 

The tool, called ParaMODA, incorporates the existing motif-centric algorithms - namely [Motif Discovery Algorithm (MODA)](http://www.ncbi.nlm.nih.gov/pubmed/20154426) and [Grochow-Kellis (GK) Algorithm)](http://compbio.mit.edu/publications/C04_Grochow_RECOMB_07.pdf) - as well as a new algorithm for carrying out the same task. The new algorithm is essentially a modification to GK's. The new algorithm performed better than GK's in all the cases tested, although the performance improvement varied with the shape of the query graphs.

More importantly the new algorithm allows for parallelization of huge chunks of the task – which will be helpful, especially for studying motifs of double-digit sizes in large networks.

The tool also (optionally) collect and store discovered instances on the disk for future retrieval and analysis.
