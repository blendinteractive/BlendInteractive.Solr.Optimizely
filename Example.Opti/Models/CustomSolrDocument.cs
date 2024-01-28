using BlendInteractive.Solr.Optimizely;
using SolrNet.Attributes;

namespace Example.Opti.Models
{
    public class CustomSolrDocument : SolrDocument
    {
        public static class CustomFieldNames
        {
            public const string Headline = "headline_s";
        }

        [SolrField(CustomFieldNames.Headline)]
        public string? Headline { get; set; }
    }
}
