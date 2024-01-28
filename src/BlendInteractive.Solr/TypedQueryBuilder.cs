using SolrNet;
using System.Linq.Expressions;

namespace BlendInteractive.Solr
{
    public class TypedQueryBuilder<TDoc> : QueryBuilder
    {
        public TypedQueryBuilder() : base() { }

        public TypedQueryBuilder(string? keyword, QueryField[]? queryFields) : base(keyword, queryFields)
        {
        }

        public TypedQueryBuilder<TDoc> Match(Expression<Func<TDoc, object>> expression, string value)
        {
            var name = expression.GetMemberSolrName();
            AddFilter(new SolrQueryByField(name, value));
            return this;
        }

        public TypedQueryBuilder<TDoc> NotMatch(Expression<Func<TDoc, object>> expression, string value)
        {
            var name = expression.GetMemberSolrName();
            AddFilter(new SolrNotQuery(new SolrQueryByField(name, value)));
            return this;
        }

        public TypedQueryBuilder<TDoc> InRange<RT>(Expression<Func<TDoc, object>> expression, RT firstValue, RT secondValue)
        {
            var name = expression.GetMemberSolrName();
            AddFilter(new SolrQueryByRange<RT>(name, firstValue, secondValue));
            return this;
        }

        public TypedQueryBuilder<TDoc> Facet(Expression<Func<TDoc, object>> expression, Action<SolrFacetFieldQuery>? adjust)
        {
            var name = expression.GetMemberSolrName();
            var facet = new SolrFacetFieldQuery(name);
            adjust?.Invoke(facet);
            AddFacet(facet);
            return this;
        }

        public TypedQueryBuilder<TDoc> Any(Expression<Func<TDoc, object>> expression, params string[] values)
        {
            var name = expression.GetMemberSolrName();
            AddFilter(new SolrQueryInList(name, values));
            return this;
        }

        public TypedQueryBuilder<TDoc> All(Expression<Func<TDoc, object>> expression, params string[] values)
        {
            var name = expression.GetMemberSolrName();
            var all = new SolrMultipleCriteriaQuery(values.Select(x => new SolrQueryByField(name, x)), SolrMultipleCriteriaQuery.Operator.AND);
            AddFilter(all);
            return this;
        }

        public TypedQueryBuilder<TDoc> OrderBy(Expression<Func<TDoc, object>> expression, Order order)
        {
            var name = expression.GetMemberSolrName();
            AddSort(name, Order.ASC);
            return this;

        }

        public TypedQueryBuilder<TDoc> OrderByAsc(Expression<Func<TDoc, object>> expression)
        {
            var name = expression.GetMemberSolrName();
            AddSort(name, Order.ASC);
            return this;
        }

        public TypedQueryBuilder<TDoc> OrderByDesc(Expression<Func<TDoc, object>> expression)
        {
            var name = expression.GetMemberSolrName();
            AddSort(name, Order.DESC);
            return this;
        }
    }
}
