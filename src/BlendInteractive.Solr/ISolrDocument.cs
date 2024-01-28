namespace BlendInteractive.Solr
{
    public interface ISolrDocument
    {
        string? Id { get; }
        double Boost { get; }
    }
}
