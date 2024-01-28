using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace BlendInteractive.Solr.Optimizely
{
    public class SolrSynchronizationUtility<TDoc> where TDoc : SolrDocument, new()
    {
        private readonly SolrDocumentService<TDoc> documentService;
        private readonly IContentRepository contentRepository;
        private readonly ILanguageBranchRepository branchRepository;

        public SolrSynchronizationUtility(
            SolrDocumentService<TDoc> documentService, 
            IContentRepository contentRepository,
            ILanguageBranchRepository branchRepository)
        {
            this.documentService = documentService;
            this.contentRepository = contentRepository;
            this.branchRepository = branchRepository;
        }

        public virtual void OnPublished(IContent content)
        {
            using var manager = ServiceLocator.Current.GetInstance<SolrOperationsManager<TDoc>>();

            AddContent(content, manager);
        }

        public virtual void AddContent(IContent content, SolrOperationsManager<TDoc> manager)
        {
            manager.Delete(
               new OptimizelyQuery<TDoc>()
                   .Match(x => x.ContentLink, content.ContentLink.ID.ToString())
                   .BuildFilterQuery());

            if (content is IExcludeFromSearch excludable && excludable.ExcludeFromSearch)
                return;

            var doc = new TDoc();
            documentService.Apply(doc, content);
            if (string.IsNullOrEmpty(doc.Id))
                return;

            manager.Add(doc);
        }

        public virtual void OnDeleted(IContent content)
        {
            if (content == null)
                return;

            // Delete all descendants of all languages of this content
            using var manager = ServiceLocator.Current.GetInstance<SolrOperationsManager<TDoc>>();
            manager.Delete(
                new OptimizelyQuery<TDoc>()
                    .DescendantOf(content.ContentLink.ID.ToString())
                    .BuildFilterQuery()
            );
        }

        public virtual void OnMoved(IContent content, ContentReference target)
        {
            // Delete all descendants of all languages of this content
            OnDeleted(content);
            if (target.CompareToIgnoreWorkID(SiteDefinition.Current.WasteBasket))
                return;

            using var manager = ServiceLocator.Current.GetInstance<SolrOperationsManager<TDoc>>();

            var languages = branchRepository.ListEnabled();

            // Reindex all the descendant content of all languages.
            var stack = new Stack<int>();
            stack.Push(content.ContentLink.ID);
            while (stack.Count > 0)
            {
                // Re-add pages under all languages
                var link = new ContentReference(stack.Pop());
                var translated = contentRepository.GetLanguageBranches<IContent>(link);
                foreach (var branch in translated)
                {
                    AddContent(branch, manager);
                }

                var ids = languages.SelectMany(lang =>
                {
                    var childrenInLanguage = contentRepository.GetChildren<IContent>(link, lang.Culture);
                    return childrenInLanguage.Select(x => x.ContentLink.ID);
                }).Distinct();

                foreach (var id in ids)
                    stack.Push(id);
            }
        }
    }
}
