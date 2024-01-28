using BlendInteractive.Solr;
using BlendInteractive.Solr.Optimizely;
using System.ComponentModel.DataAnnotations;

namespace Example.Opti.Models.Pages
{
    [ContentType(
        DisplayName = "Search Page",
        GUID = "4a44aa81-5baa-4ca2-abee-3441c6e2d6b7",
        Description = "Search page")]
    public class SearchPage : AbstractContentPage, IExcludeFromSearch
    {
        [CultureSpecific]
        [Display(
            Name = "Body",
            Description = "",
            Order = 10,
            GroupName = SystemTabNames.Content)]
        public virtual XhtmlString? Body { get; set; }

        public bool ExcludeFromSearch => true;
    }
}
