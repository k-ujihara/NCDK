namespace FaulonSignatures
{
    public interface VisitableDAG
    {
        void Accept(DAGVisitor visitor);
    }
}
