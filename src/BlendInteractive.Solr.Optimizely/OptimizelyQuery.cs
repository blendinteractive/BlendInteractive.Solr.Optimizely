using EPiServer.Core;
using SolrNet;

namespace BlendInteractive.Solr.Optimizely
{
    public class OptimizelyQuery<TDoc> : TypedQueryBuilder<TDoc> where TDoc : SolrDocument
    {
        public static string? EveryoneAcl { get; set; } = "Everyone";

        public OptimizelyQuery(string query, params QueryField[] fields) : base(query, fields) { }

        public OptimizelyQuery() :base() { }

        public OptimizelyQuery<TDoc> WithDefaults() => Access().Published();

        public OptimizelyQuery<TDoc> PagesOnly() => OfType<PageData>();
        public OptimizelyQuery<TDoc> BlocksOnly() => OfType<BlockData>();


        public OptimizelyQuery<TDoc> Access(IEnumerable<string>? users = null)
        {
            if (users is null && EveryoneAcl is not null)
            {
                AddFilter(new SolrQueryByField(SolrDocument.FieldNames.ACL, EveryoneAcl));
            }
            else
            {
                AddFilter(new SolrQueryInList(SolrDocument.FieldNames.ACL, users));
            }
            return this;
        }

        public OptimizelyQuery<TDoc> InLanguage(string language)
        {
            Match(x => x.Language!, language);
            return this;
        }

        public OptimizelyQuery<TDoc> WithinSite(Guid siteId)
        {
            Match(x => x.SiteId!, siteId.ToString());
            return this;
        }

        public OptimizelyQuery<TDoc> Published(DateTime? publishedDate = null)
        {
            DateTime filterDate = publishedDate.HasValue ? publishedDate.Value : DateTime.Now;
            AddFilter(new SolrQueryByRange<DateTime>(SolrDocument.FieldNames.StartPublish, DateTime.MinValue, filterDate, true));
            AddFilter(new SolrQueryByRange<DateTime>(SolrDocument.FieldNames.StopPublish, filterDate, DateTime.MaxValue, true));
            return this;
        }

        public OptimizelyQuery<TDoc> IncludeTypesExact(params string[] types)
        {
            AddFilter(new SolrQueryInList(SolrDocument.FieldNames.ContentTypeName, types));
            return this;
        }

        public OptimizelyQuery<TDoc> OfTypeExact<TType>()
        {
            AddFilter(new SolrQueryByField(SolrDocument.FieldNames.ContentTypeName, typeof(TType).Name));
            return this;
        }

        public OptimizelyQuery<TDoc> ExcludeTypesExact(params string[] types)
        {
            AddFilter(!new SolrQueryInList(SolrDocument.FieldNames.ContentTypeName, types));
            return this;
        }

        public OptimizelyQuery<TDoc> IncludeTypes(params string[] types)
        {
            AddFilter(new SolrQueryInList(SolrDocument.FieldNames.InheritedTypes, types));
            return this;
        }

        public OptimizelyQuery<TDoc> OfType<TType>()
        {
            AddFilter(new SolrQueryByField(SolrDocument.FieldNames.InheritedTypes, typeof(TType).Name));
            return this;
        }

        public OptimizelyQuery<TDoc> ExcludeTypes(params string[] types)
        {
            AddFilter(!new SolrQueryInList(SolrDocument.FieldNames.InheritedTypes, types));
            return this;
        }

        public OptimizelyQuery<TDoc> MatchAnyCategory(IEnumerable<int> categoryIds)
        {
            AddFilter(new SolrQueryInList(SolrDocument.FieldNames.Categories, categoryIds.Select(x => x.ToString())));
            return this;
        }

        public OptimizelyQuery<TDoc> MatchAllCategories(IEnumerable<int> categoryIds)
        {
            AddFilter(new SolrMultipleCriteriaQuery(categoryIds.Select(x => new SolrQueryByField(SolrDocument.FieldNames.Categories, x.ToString())), SolrMultipleCriteriaQuery.Operator.AND));
            return this;
        }

        public OptimizelyQuery<TDoc> DescendantOf(ContentReference link)
            => DescendantOf(link.ID.ToString());

        public OptimizelyQuery<TDoc> DescendantOf(string id)
        {
            AddFilter(new SolrQueryInList(SolrDocument.FieldNames.AncestorIds, id));
            return this;
        }

        public OptimizelyQuery<TDoc> ChildOf(ContentReference link)
            => ChildOf(link.ID.ToString());

        public OptimizelyQuery<TDoc> ChildOf(string id)
        {
            AddFilter(new SolrQueryByField(SolrDocument.FieldNames.ParentId, id));
            return this;
        }

        public OptimizelyQuery<TDoc> ExcludeIds(IEnumerable<string> ids)
        {
            AddFilter(!new SolrQueryInList(SolrDocument.FieldNames.ContentLink, ids));
            return this;
        }

        public OptimizelyQuery<TDoc> RequireIds(IEnumerable<string> ids)
        {
            AddFilter(new SolrQueryInList(SolrDocument.FieldNames.ContentLink, ids));
            return this;
        }
    }
}
