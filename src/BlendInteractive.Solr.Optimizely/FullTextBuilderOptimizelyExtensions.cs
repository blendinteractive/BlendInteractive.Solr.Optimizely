using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace BlendInteractive.Solr.Optimizely
{
    public static class FullTextBuilderOptimizelyExtensions
    {
        public static T AddHtml<T>(this T builder, XhtmlString? htmlString) where T : FullTextBuilder
        {
            if (htmlString is null)
                return builder;

            return builder.AddHtml(htmlString.ToInternalString());
        }

        public static T AddContentList<T>(this T builder, IList<ContentReference>? contentList) where T : FullTextBuilder
        {
            if (contentList is null || !contentList.Any())
                return builder;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            foreach (var item in contentList)
            {
                if (contentLoader.TryGet(item, out IContent content) && content is IHaveFullText fullText)
                {
                    builder = fullText.AddContent(builder);
                }
            }

            return builder;
        }

        public static T AddContentArea<T>(this T builder, ContentArea? contentArea) where T : FullTextBuilder
        {
            if (contentArea is null)
                return builder;

            var items = contentArea.FilteredItems;
            if (items == null)
                return builder;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            foreach (var item in items)
            {
                if (contentLoader.TryGet(item.ContentLink, out IContent content) && content is IHaveFullText fullText)
                {
                    builder = fullText.AddContent(builder);
                }
            }

            return builder;
        }

        public static T AddBlock<T>(this T builder, ContentData? block) where T : FullTextBuilder
        {
            if (block is null)
                return builder;

            if (block is IHaveFullText content)
            {
                builder = content.AddContent(builder);
            }

            return builder;
        }
    }
}
