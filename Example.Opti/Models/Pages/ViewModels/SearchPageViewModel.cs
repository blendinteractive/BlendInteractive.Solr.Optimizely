using SolrNet;

namespace Example.Opti.Models.Pages.ViewModels
{
    public class SearchPageViewModel
    {
        public SolrQueryResults<CustomSolrDocument>? Results { get; internal set; }
        public string? Query { get; internal set; }
    }
}
