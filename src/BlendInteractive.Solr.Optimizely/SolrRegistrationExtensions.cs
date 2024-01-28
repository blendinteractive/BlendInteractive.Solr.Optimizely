using Microsoft.Extensions.DependencyInjection;

namespace BlendInteractive.Solr.Optimizely
{
    public static class SolrRegistrationExtensions
    {

        public static IServiceCollection AddOptimizelySolr<TDoc>(this IServiceCollection services) where TDoc: SolrDocument, new()
        {
            services.AddTransient<SolrSynchronizationUtility<TDoc>>();
            services.AddTransient<SolrDocumentService<TDoc>>();
            services.AddTransient<SolrOperationsManager<TDoc>>();
            services.AddTransient<SolrSearchService<TDoc>>();

            return services;
        }
    }
}
