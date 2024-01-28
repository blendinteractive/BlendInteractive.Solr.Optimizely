using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Scheduler;
using EPiServer.Web;

namespace BlendInteractive.Solr.Optimizely
{
    public abstract class AbstractSolrReindexScheduledJob<TDoc> : ScheduledJobBase where TDoc : SolrDocument, new()
    {
        private readonly SolrSynchronizationUtility<TDoc> synchronizationUtility;
        private readonly ISiteDefinitionRepository siteDefinitionRepository;
        private readonly IContentLoader contentLoader;
        private readonly ILanguageBranchRepository languageBranchRepository;

        public AbstractSolrReindexScheduledJob(
            SolrSynchronizationUtility<TDoc> synchronizationUtility, 
            ISiteDefinitionRepository siteDefinitionRepository,
            IContentLoader contentLoader,
            ILanguageBranchRepository languageBranchRepository)
        {
            this.synchronizationUtility = synchronizationUtility;
            this.siteDefinitionRepository = siteDefinitionRepository;
            this.contentLoader = contentLoader;
            this.languageBranchRepository = languageBranchRepository;
        }

        public override string Execute()
        {
            var languages = languageBranchRepository.ListEnabled();

            foreach (var site in siteDefinitionRepository.List())
            {
                var content = languages
                    .Select(x => contentLoader.TryGet(site.StartPage, out IContent content) ? content : null)
                    .Where(x => x is not null)
                    .FirstOrDefault();

                if (content is null)
                    continue;

                // Fake a "move" to force re-indexing of this page and all descendant content
                synchronizationUtility.OnMoved(content, content.ContentLink);
            }

            return "OK";
        }
    }
}
