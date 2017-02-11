namespace NCDK.Graphs.Matrix
{
    /**
     * Calculator for a topological matrix representation of this AtomContainer. An
     * topological matrix is a matrix of quare NxN matrix, where N is the number of
     * atoms in the AtomContainer. The element i,j of the matrix is the distance between
     * two atoms in a molecule.
     *
     * @author federico
     * @cdk.githash
     * @cdk.module  qsarmolecular
     */
    public class TopologicalMatrix : IGraphMatrix
    {
        /**
         * Returns the topological matrix for the given AtomContainer.
         *
         * @param  container The AtomContainer for which the matrix is calculated
         * @return           A topological matrix representating this AtomContainer
         */
        public static int[][] GetMatrix(IAtomContainer container)
        {
            int[][] conMat = AdjacencyMatrix.GetMatrix(container);
            int[][] topolDistance = PathTools.ComputeFloydAPSP(conMat);

            return topolDistance;
        }
    }
}
