using SolrNet;
using SolrNet.Commands.Parameters;

namespace BlendInteractive.Solr
{
    public record struct QueryField(string Name, double Boost);

    public class QueryBuilder
    {
        private readonly List<string> selectColumns = new List<string>();
        private readonly List<ISolrQuery> filters = new List<ISolrQuery>();
        private readonly List<ISolrFacetQuery> facets = new List<ISolrFacetQuery>();
        private readonly List<(string, Order)> sort = new List<(string, Order)>();

        private readonly string? keyword;
        private readonly QueryField[]? queryFields;

        public QueryBuilder(string? keyword, QueryField[]? queryFields)
        {
            this.keyword = keyword;
            this.queryFields = queryFields;
        }

        public QueryBuilder() : this(null, null) { }

        public void AddFilter(ISolrQuery query) => filters.Add(query);
        public void AddFacet(ISolrFacetQuery facet) => facets.Add(facet);
        public void AddSort(string key, Order order) => sort.Add((key, order));
        public void SelectColumn(string column) => selectColumns.Add(column);
        public void SelectColumns(params string[] columns) => selectColumns.AddRange(columns);

        public virtual ISolrQuery BuildQuery()
        {
            var localParams = new LocalParams();

            ISolrQuery query;
            if (keyword is not null && queryFields is not null)
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = new SolrQuery(keyword);
                }
                else
                {
                    query = new SolrHasValueQuery(Constants.IdentifierFieldName);
                }
                localParams.Add("type", "edismax");
                localParams.Add("qf", string.Join(" ", queryFields.Select((field) => $"{field.Name}^{field.Boost}")));
            }
            else
            {
                query = new SolrHasValueQuery(Constants.IdentifierFieldName);
            }

            return localParams + query;
        }

        public ISolrQuery BuildFilterQuery()
        {
            if (filters.Count == 0)
                return new SolrHasValueQuery(Constants.IdentifierFieldName);

            if (filters.Count == 0)
                return filters[0];

            return new SolrMultipleCriteriaQuery(filters, SolrMultipleCriteriaQuery.Operator.AND);
        }

        public virtual QueryOptions BuildOptions(Limit limit)
        {
            var options = new QueryOptions();
            options.FilterQueries = options.FilterQueries ?? new List<ISolrQuery>();
            foreach (var filter in filters)
            {
                options.FilterQueries.Add(filter);
            }

            if (facets.Any())
            {
                options.Facet = new FacetParameters()
                {
                    Queries = facets
                };
            }

            if (selectColumns is not null && selectColumns.Count > 0)
            {
                options.AddFields(selectColumns.ToArray());
            }
            else
            {
                options.AddFields("*");
            }

            foreach (var (key, order) in sort)
            {
                options.AddOrder(new SortOrder(key, order));
            }

            options.StartOrCursor = new StartOrCursor.Start(limit.Skip);
            options.Rows = limit.Take;

            return options;
        }

        public virtual QueryBuilder Clone()
        {
            var clone = new QueryBuilder(keyword, queryFields);
            Apply(clone);
            return clone;
        }

        protected void Apply(QueryBuilder clone)
        {
            foreach (var filter in filters)
                clone.AddFilter(filter);
            foreach (var facet in facets)
                clone.AddFacet(facet);
            foreach (var s in sort)
                clone.sort.Add(s);
            foreach (var column in selectColumns)
                clone.selectColumns.Add(column);
        }
    }
}
