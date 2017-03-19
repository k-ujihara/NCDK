using System.Collections.Generic;
using System.Text;

namespace NCDK.FaulonSignatures
{
    /// <summary>
    /// A signature string is read back in as a colored tree, not a DAG since some 
    /// of the information in the DAG is lost when printing out. A colored tree can
    /// be reconstructed into a graph.
    /// </summary>
    // @author maclean
    public class ColoredTree
    {

        public class Node : VisitableColoredTree
        {

            public List<Node> children = new List<Node>();

            public readonly string label;

            public readonly string edgeLabel;

            public readonly Node parent;

            public readonly int color;

            public readonly int height;

            public Node(string label, Node parent, int height)
            {
                this.label = label;
                this.parent = parent;
                this.color = -1;
                this.height = height;
                this.edgeLabel = "";
                if (parent != null)
                {
                    parent.children.Add(this);
                }
            }

            public Node(string label, Node parent, int height, int color)
            {
                this.label = label;
                this.parent = parent;
                this.color = color;
                this.height = height;
                this.edgeLabel = "";
                if (parent != null)
                {
                    parent.children.Add(this);
                }
            }

            public Node(string label, Node parent,
                    int height, int color, string edgeLabel)
            {
                this.label = label;
                this.parent = parent;
                this.color = color;
                this.height = height;
                this.edgeLabel = edgeLabel;
                if (parent != null)
                {
                    parent.children.Add(this);
                }
            }

            public void Accept(ColoredTreeVisitor visitor)
            {
                visitor.Visit(this);
                foreach (var child in this.children)
                {
                    child.Accept(visitor);
                }
            }

            public bool IsColored()
            {
                return this.color != -1;
            }

            public void BuilderString(StringBuilder builder)
            {
                if (this.IsColored())
                {
                    builder.Append(this.edgeLabel);
                    builder.Append("[").Append(this.label);
                    builder.Append(",").Append(this.color).Append("]");
                }
                else
                {
                    builder.Append(this.edgeLabel);
                    builder.Append("[").Append(this.label).Append("]");
                }
                if (this.children.Count > 0) { builder.Append("("); }
                foreach (var child in this.children)
                {
                    child.BuilderString(builder);
                }
                if (this.children.Count > 0) { builder.Append(")"); }
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                this.BuilderString(builder);
                return builder.ToString();
            }
        }

        private int height;

        private Node root;

        private int maxColor;

        public ColoredTree(string rootLabel)
        {
            this.root = new Node(rootLabel, null, 1);
            this.height = 1;
        }

        public void Accept(ColoredTreeVisitor visitor)
        {
            this.root.Accept(visitor);
        }

        public Node GetRoot()
        {
            return this.root;
        }

        public Node MakeNode(string label, Node parent, int currentHeight, int color)
        {
            if (color > maxColor)
            {
                maxColor = color;
            }
            return new Node(label, parent, currentHeight, color);
        }

        public Node MakeNode(string label, Node parent, int currentHeight,
                int color, string edgeSymbol)
        {
            if (color > maxColor)
            {
                maxColor = color;
            }
            return new Node(label, parent, currentHeight, color, edgeSymbol);
        }

        public void UpdateHeight(int height)
        {
            if (height > this.height)
            {
                this.height = height;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.root.BuilderString(builder);
            return builder.ToString();
        }

        public int Height => this.height;

        public int NumberOfColors()
        {
            return maxColor;
        }
    }
}
