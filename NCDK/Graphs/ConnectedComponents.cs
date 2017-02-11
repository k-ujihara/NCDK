using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NCDK.Common.Collections;

namespace NCDK.Graphs
{
    /**
     * Compute the connected components of an adjacency list.
     *
     * <blockquote><pre>
     *     int[][]             g          = GraphUtil.ToAdjList(Container(l
     *     ConnectedComponents cc         = new ConnectedComponents(g);
     *     int[]               components = cc.Components();
     *     for (int v = 0; v < g.Length; v++)
     *         components[v];
     * </pre></blockquote>
     *
     * @author John May
     * @cdk.module core
     * @cdk.githash
     */
    public sealed class ConnectedComponents
    {
        /// <summary>Adjacency-list representation of a graph.</summary>
        private readonly int[][] g;

        /// <summary>Stores the component of each vertex.</summary>
        private readonly int[] component;

        /// <summary>The number of components.</summary>
        private int components;

        /// <summary>The number remaining vertices.</summary>
        private int remaining;

        /**
         * Compute the connected components of an adjacency list, {@code g}.
         *
         * @param g graph (adjacency list representation)
         */
        public ConnectedComponents(int[][] g)
        {
            this.g = g;
            this.component = new int[g.Length];
            this.remaining = g.Length;
            for (int i = 0; remaining > 0 && i < g.Length; i++)
                if (component[i] == 0) Visit(i, ++components);
        }

        /**
         * Visit a vertex and mark it a member of component {@code c}.
         *
         * @param v vertex
         * @param c component
         */
        private void Visit(int v, int c)
        {
            remaining--;
            component[v] = c;
            foreach (var w in g[v])
                if (component[w] == 0) Visit(w, c);
        }

        /**
         * Access the components each vertex belongs to.
         *
         * @return component labels
         */
        public int[] Components()
        {
            return Arrays.CopyOf(component, component.Length);
        }

        public int NComponents => components;
    }
}
