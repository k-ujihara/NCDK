namespace NCDK.FaulonSignatures
{
    public interface DAGVisitor
    {
         void Visit(DAG.Node node);
    }
}
