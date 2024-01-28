using SolrNet;
using SolrNet.Commands.Parameters;

namespace BlendInteractive.Solr.Optimizely
{
    public class SolrSearchService<TDoc> where TDoc : SolrDocument
    {
        private readonly ISolrOperations<TDoc> solr;

        public SolrSearchService(ISolrOperations<TDoc> solr)
        {
            this.solr = solr;
        }

        public virtual SolrQueryResults<TDoc> ExecuteQuery<TQuery>(TQuery query, Limit limit, Action<QueryOptions>? adjustOptions = null) where TQuery : OptimizelyQuery<TDoc>
        {
            var solrQuery = query.BuildQuery();
            var options = query.BuildOptions(limit);

            adjustOptions?.Invoke(options);

            var results = solr.Query(solrQuery, options);

            return results;
        }
    }
}
