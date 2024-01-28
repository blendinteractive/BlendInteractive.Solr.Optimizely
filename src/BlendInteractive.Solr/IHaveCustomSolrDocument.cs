namespace BlendInteractive.Solr
{
    public interface IHaveCustomSolrDocument<TDoc>
    {
        void ApplyTo(TDoc doc);
    }
}
