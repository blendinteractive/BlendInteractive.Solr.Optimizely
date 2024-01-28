using BlendInteractive.Solr;
using BlendInteractive.Solr.Optimizely;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Example.Opti.Models;
using Example.Opti.Models.Pages;
using Example.Opti.Models.Pages.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Example.Opti.Controllers.Pages
{
    public class SearchPageController : PageController<SearchPage>
    {
        public const int PageWeight = 5;

        private readonly SolrSearchService<CustomSolrDocument> searchService;

        public SearchPageController(SolrSearchService<CustomSolrDocument> searchService)
        {
            this.searchService = searchService;
        }

        public IActionResult Index(SearchPage currentContent, string? q = "", int page = 0)
        {
            var viewModel = new SearchPageViewModel();
            viewModel.Query = q;

            if (!string.IsNullOrWhiteSpace(q))
            {
                // Query the full text and Headline, boosting headline a bit.
                var query = new OptimizelyQuery<CustomSolrDocument>(q, 
                    new QueryField(SolrDocument.FieldNames.Text, 1),
                    new QueryField(CustomSolrDocument.CustomFieldNames.Headline, 2)
                );

                // Only published, public pages, in the current language, under the current site.
                query
                    .WithDefaults()
                    .InLanguage(currentContent.Language.Name)
                    .PagesOnly()
                    .WithinSite(SiteDefinition.Current.Id);

                // Pagination
                page = Math.Max(0, page);
                var limit = new Limit(page * PageWeight, PageWeight);

                var results = searchService.ExecuteQuery(query, limit);
                viewModel.Results = results;
            }

            return View("~/Views/Pages/SearchPage.cshtml", viewModel);
        }
    }
}
