using BlendInteractive.Solr.Optimizely;
using EPiServer.PlugIn;
using EPiServer.Web;
using Example.Opti.Models;

namespace Example.Opti.Business.Solr
{

    [ScheduledPlugIn(
        DisplayName = "Reindex all content in Solr",
        DefaultEnabled = true,
        GUID = "eee2bc6d-2be1-4d35-8b1c-166c39bf4671")]
    public class SolrReindexScheduledJob : AbstractSolrReindexScheduledJob<CustomSolrDocument>
    {
        public SolrReindexScheduledJob(SolrSynchronizationUtility<CustomSolrDocument> synchronizationUtility, ISiteDefinitionRepository siteDefinitionRepository, IContentLoader contentLoader, ILanguageBranchRepository languageBranchRepository) : base(synchronizationUtility, siteDefinitionRepository, contentLoader, languageBranchRepository)
        {
        }
    }
}
