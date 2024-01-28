using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace Example.Opti.Models.Blocks
{
    [ContentType(
        DisplayName = "Raw Code Block",
        GUID = "758a16db-776a-4ea4-8516-405a19fcff1d",
        Description = "")]
    public class RawCodeBlock : BlockData
    {
        [CultureSpecific]
        [Display(
            Name = "Code",
            Description = "",
            Order = 10,
            GroupName = SystemTabNames.Content)]
        [UIHint(UIHint.Textarea)]
        public virtual string? Code { get; set; }
    }
}
