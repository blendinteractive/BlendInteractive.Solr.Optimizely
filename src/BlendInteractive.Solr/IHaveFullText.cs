namespace BlendInteractive.Solr
{
    public interface IHaveFullText
    {
        T AddContent<T>(T builder) where T : FullTextBuilder;
    }
}
