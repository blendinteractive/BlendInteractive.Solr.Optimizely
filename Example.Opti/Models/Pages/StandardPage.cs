﻿using BlendInteractive.Solr;
using BlendInteractive.Solr.Optimizely;
using System.ComponentModel.DataAnnotations;

namespace Example.Opti.Models.Pages
{
    [ContentType(
        DisplayName = "Standard Page",
        GUID = "41409c2b-f663-4722-ac3f-10a6c9a9100d",
        Description = "")]
    public class StandardPage : AbstractContentPage, IHaveFullText, IHaveCustomSolrDocument<CustomSolrDocument>
    {
        [CultureSpecific]
        [Display(
            Name = "Headline",
            Description = "The headline to appear at the top of the page",
            Order = 10,
            GroupName = SystemTabNames.Content)]
        public virtual string? Headline { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Body",
            Description = "",
            Order = 20,
            GroupName = SystemTabNames.Content)]
        public virtual XhtmlString? Body { get; set; }

        [CultureSpecific]
        [Display(
            Name = "Content",
            Description = "",
            Order = 30,
            GroupName = SystemTabNames.Content)]
        public virtual ContentArea? Content { get; set; }

        public T AddContent<T>(T builder) where T : FullTextBuilder
            => builder
                .AddText(Headline)
                .AddHtml(Body)
                .AddContentArea(Content);

        void IHaveCustomSolrDocument<CustomSolrDocument>.ApplyTo(CustomSolrDocument doc)
        {
            doc.Headline = Headline;
        }
    }
}
