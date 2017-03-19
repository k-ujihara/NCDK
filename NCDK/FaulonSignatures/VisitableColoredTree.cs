namespace NCDK.FaulonSignatures
{
    public interface VisitableColoredTree
    {
        void Accept(ColoredTreeVisitor visitor);
    }
}
