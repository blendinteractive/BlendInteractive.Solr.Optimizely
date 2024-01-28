using EPiServer.Core;
using SolrNet.Attributes;

namespace BlendInteractive.Solr.Optimizely
{
    public class SolrDocument : ISolrDocument
    {
        public const string RepositoryName = "Optimizely";

        public static class FieldNames
        {
            public const string Id = "id";
            public const string Score = "score";
            public const string ContentGuid = "ContentGuid_s";
            public const string ContentLink = "ContentLink_s";
            public const string SiteId = "SiteId_s";
            public const string ParentId = "ParentId_i";
            public const string AncestorIds = "AncestorIds_is";
            public const string ContentTypeName = "ContentTypeName_s";
            public const string CreatedBy = "CreatedBy_s";
            public const string Created = "Created_dt";
            public const string StartPublish = "StartPublish_dt";
            public const string StopPublish = "StopPublish_dt";
            public const string Title = "Title_txt_en";
            public const string LinkUrl = "LinkUrl_s";
            public const string Language = "Language_s";
            public const string ACL = "ACL_ss";
            public const string VisibleInMenu = "VisibleInMenu_b";
            public const string Categories = "Categories_is";
            public const string CategoryNames = "CategoryNames_ss";
            public const string SiteUrl = "SiteUrl_s";
            public const string Status = "Status_s";
            public const string SortIndex = "SortIndex_s";
            public const string InheritedTypes = "InheritedTypes_ss";
            public const string ReferencedTypes = "ReferencedTypes_ss";
            public const string ReferencedIds = "ReferencedIds_is";
            public const string Text = "text_txt_en";
            public const string Location = "Location_p";
            public const string Latitude = "Location_0_d";
            public const string Longitude = "Location_1_d";
            public const string Miles = "Miles_d";
            public const string IsShortcutPage = "IsShortcutPage_b";
        }

        [SolrUniqueKey(FieldNames.Id)]
        public virtual string? Id { get; set; }

        public SolrIdentifier? Identifier {
            get {
                if (SolrIdentifier.TryParse(Id, out var identifier))
                    return identifier;
                return default;
            }
            set
            {
                if (value is not null)
                    Id = value.ToString();
                else
                    Id = default;
            }
        }

        [SolrField(FieldNames.Score)]
        public virtual double? Score { get; set; }

        [SolrField(FieldNames.ContentGuid)]
        public virtual string? ContentGuid { get; set; }

        [SolrField(FieldNames.ContentLink)]
        public virtual int ContentLink { get; set; }

        [SolrField(FieldNames.SortIndex)]
        public virtual int SortIndex { get; set; }

        [SolrField(FieldNames.SiteId)]
        public virtual string? SiteId { get; set; }

        [SolrField(FieldNames.ParentId)]
        public virtual int ParentId { get; set; }

        [SolrField(FieldNames.AncestorIds)]
        public virtual ICollection<int> AncestorIds { get; set; } = new HashSet<int>();


        [SolrField(FieldNames.ContentTypeName)]
        public virtual string? ContentTypeName { get; set; }

        [SolrField(FieldNames.CreatedBy)]
        public virtual string? CreatedBy { get; set; }

        [SolrField(FieldNames.Created)]
        public virtual DateTime Created { get; set; }

        [SolrField(FieldNames.StartPublish)]
        public virtual DateTime? StartPublish { get; set; }

        [SolrField(FieldNames.StopPublish)]
        public virtual DateTime? StopPublish { get; set; }

        [SolrField(FieldNames.Title)]
        public virtual string? Title { get; set; }

        [SolrField(FieldNames.LinkUrl)]
        public virtual string? LinkUrl { get; set; }

        [SolrField(FieldNames.Language)]
        public virtual string? Language { get; set; }

        [SolrField(FieldNames.ACL)]
        public virtual ICollection<string> ACL { get; set; } = new HashSet<string>();

        [SolrField(FieldNames.VisibleInMenu)]
        public virtual bool VisibleInMenu { get; set; }

        [SolrField(FieldNames.Categories)]
        public virtual ICollection<int> Categories { get; set; } = new HashSet<int>();

        [SolrField(FieldNames.CategoryNames)]
        public virtual ICollection<string> CategoryNames { get; set; } = new HashSet<string>();

        [SolrField(FieldNames.SiteUrl)]
        public virtual string? SiteUrl { get; set; }

        [SolrField(FieldNames.Status)]
        public string? Status { get; set; }

        [SolrField(FieldNames.InheritedTypes)]
        public ICollection<string> InheritedTypes { get; set; } = new HashSet<string>();

        #region Location

        [SolrField(FieldNames.Location)]
        public SolrNet.Location? Location { get; set; }

        [SolrField(FieldNames.Latitude)]
        public double? Latitude { get; set; }

        [SolrField(FieldNames.Longitude)]
        public double? Longitude { get; set; }

        [SolrField(FieldNames.Miles)]
        public double Miles { get; set; }

        #endregion Location

        [SolrField(FieldNames.Text)]
        public virtual string? Text { get; set; }

        public double Boost { get; set; }

        [SolrField(FieldNames.IsShortcutPage)]
        public virtual bool IsShortcutPage { get; set; }
    }
}
