using BlendInteractive.Solr;
using BlendInteractive.Solr.Optimizely;
using System.ComponentModel.DataAnnotations;

namespace Example.Opti.Models.Blocks
{
    [ContentType(
        DisplayName = "Editorial Block",
        GUID = "aa40f435-d1fb-4094-9625-41c8e101e2cc",
        Description = "")]
    public class EditorialBlock : BlockData, IHaveFullText
    {
        [CultureSpecific]
        [Display(
            Name = "Body",
            Description = "",
            Order = 10,
            GroupName = SystemTabNames.Content)]
        public virtual XhtmlString? Body { get; set; }

        public T AddContent<T>(T builder) where T : FullTextBuilder
            => builder.AddHtml(Body);
    }
}
