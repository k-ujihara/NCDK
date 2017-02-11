namespace FaulonSignatures
{
    public interface VisitableColoredTree
    {
        void Accept(ColoredTreeVisitor visitor);
    }
}
