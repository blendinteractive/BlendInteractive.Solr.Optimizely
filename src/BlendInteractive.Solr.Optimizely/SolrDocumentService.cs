using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using EPiServer.Web;

namespace BlendInteractive.Solr.Optimizely
{
    public class SolrDocumentService<TDoc> where TDoc : SolrDocument
    {
        private readonly IContentLoader contentLoader;
        private readonly CategoryRepository categoryRepository;
        private readonly ISiteDefinitionResolver siteDefinitionResolver;

        private static readonly DateTime MaxDate = new DateTime(2100, 1, 1);

        public SolrDocumentService(IContentLoader contentLoader, CategoryRepository categoryRepository, ISiteDefinitionResolver siteDefinitionResolver)
        {
            this.contentLoader = contentLoader;
            this.categoryRepository = categoryRepository;
            this.siteDefinitionResolver = siteDefinitionResolver;
        }

        public virtual void Apply(TDoc doc, IContent content)
        {
            switch (content)
            {
                case PageData page:
                    var parentPage = contentLoader.TryGet(page.ParentLink, out IContent parentContent) ? parentContent as PageData : default;
                    Apply(doc, page, parentPage);
                    break;
                case BlockData block:
                    Apply(doc, block);
                    break;
            }

            if (content is IHaveCustomSolrDocument<TDoc> haveCustomDoc)
            {
                haveCustomDoc.ApplyTo(doc);
            }
        }

        public virtual void Apply(TDoc doc, PageData page, PageData? parentPage)
        {
            var site = siteDefinitionResolver.GetByContent(page.ContentLink, true);

            doc.Identifier = new SolrIdentifier(page.ContentLink.ID.ToString(), SolrDocument.RepositoryName, page.Language.Name);
            doc.ContentGuid = page.ContentGuid.ToString();
            doc.ContentLink = page.ContentLink.ID;

            var sort = 0;
            if (parentPage != null && !parentPage.ContentLink.CompareToIgnoreWorkID(ContentReference.RootPage) && int.TryParse(SortingUtility.GetSortValue(page, parentPage), out sort))
            {
                doc.SortIndex = sort;
            }

            doc.SiteId = site.Id.ToString();
            doc.SiteUrl = site.SiteUrl.ToString();
            doc.ParentId = page.ParentLink.ID;
            doc.AncestorIds = contentLoader.GetAncestors(page.ContentLink).Select(x => x.ContentLink.ID).ToList();
            doc.ContentTypeName = page.GetOriginalType().Name;
            doc.CreatedBy = page.CreatedBy;
            doc.Created = page.Created;
            doc.StartPublish = page.StartPublish;
            doc.StopPublish = page.StopPublish.HasValue ? page.StopPublish.Value : MaxDate;
            doc.Title = page.Name;
            doc.LinkUrl = page.LinkURL;
            doc.Language = page.Language.Name;
            doc.ACL = page.ACL.Entries.Where(ace => (ace.Access & AccessLevel.Read) == AccessLevel.Read).Select(ace => ace.Name).ToList();
            doc.VisibleInMenu = page.VisibleInMenu;
            doc.Categories = page.Category.Select(x => x).ToList();
            doc.CategoryNames = page.Category.Select(x => categoryRepository.Get(x).Name).ToList();
            doc.Status = page.Status.ToString();
            doc.InheritedTypes = GetInheritanceHierarchy(page.GetOriginalType())
                .Select(x => x.Name)
                .ToList();
            doc.IsShortcutPage = page.LinkType != PageShortcutType.Normal;

            ApplyText(doc, page);
        }

        public virtual void Apply(TDoc doc, BlockData block)
        {
            if (block is not IContent content)
                return;

            string language = (block is ILocalizable localizable) ? localizable.Language.Name : "-";

            doc.Identifier = new SolrIdentifier(content.ContentLink.ID.ToString(), SolrDocument.RepositoryName, language);
            doc.ContentGuid = content.ContentGuid.ToString();
            doc.ContentLink = content.ContentLink.ID;
            
            doc.ContentTypeName = block.GetOriginalType().Name;

            if (block is IChangeTrackable changeTrackable)
            {
                doc.CreatedBy = changeTrackable.CreatedBy;
                doc.Created = changeTrackable.Created;
            }

            if (block is IVersionable versionable)
            {
                doc.StartPublish = versionable.StartPublish;
                doc.Status = versionable.Status.ToString();
            }

            doc.Title = content.Name;
            doc.Language = language;

            doc.InheritedTypes = GetInheritanceHierarchy(block.GetOriginalType())
                .Select(x => x.Name)
                .ToList();

            ApplyText(doc, content);
        }

        protected virtual void ApplyText(TDoc doc, IContent content)
        {
            var fullTextBuilder = new FullTextBuilder();
            if (content is IHaveFullText fullText)
            {
                fullText.AddContent(fullTextBuilder);
            }
            doc.Text = fullTextBuilder.Buffer.ToString();
        }

        private static IEnumerable<Type> GetInheritanceHierarchy(Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }

    }
}
