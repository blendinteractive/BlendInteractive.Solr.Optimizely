using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace BlendInteractive.Solr.Optimizely
{
    public static class SolrContentEvents
    {

        public static IContentEvents SynchronizeSolr<TDoc>(this IContentEvents contentEvents) where TDoc : SolrDocument, new()
        {
            contentEvents.PublishedContent += (sender, e) =>
            {
                var handler = ServiceLocator.Current.GetInstance<SolrSynchronizationUtility<TDoc>>();
                handler.OnPublished(e.Content);
            };

            contentEvents.DeletedContent += (sender, e) =>
            {
                var handler = ServiceLocator.Current.GetInstance<SolrSynchronizationUtility<TDoc>>();
                handler.OnDeleted(e.Content);
            };

            contentEvents.MovedContent += (sender, e) =>
            {
                var handler = ServiceLocator.Current.GetInstance<SolrSynchronizationUtility<TDoc>>();
                handler.OnMoved(e.Content, e.TargetLink);
            };

            return contentEvents;
        }
    }
}
