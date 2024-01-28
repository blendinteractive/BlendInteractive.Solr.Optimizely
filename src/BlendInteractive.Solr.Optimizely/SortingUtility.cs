using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;

namespace BlendInteractive.Solr.Optimizely
{
    internal static class SortingUtility
    {
        static readonly DateTime MinDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly DateTime MaxDate = new DateTime(2079, 6, 6, 0, 0, 0, DateTimeKind.Utc);

        public static string GetSortValue(PageData page, PageData? parent)
        {
            if (parent is null)
                return PeerSortOrder(page);

            var parentSort = (FilterSortOrder)parent.GetPropertyValue<int>(MetaDataProperties.PageChildOrderRule);
            return parentSort switch
            {
                FilterSortOrder.Alphabetical => Normalize(page.Name),
                FilterSortOrder.ChangedDescending => DateDescending(page.Changed),
                FilterSortOrder.CreatedAscending => DateAscending(page.Created),
                FilterSortOrder.CreatedDescending => DateDescending(page.Changed),
                FilterSortOrder.Index => PeerSortOrder(page),
                FilterSortOrder.PublishedAscending => DateAscending(page.StartPublish.HasValue ? page.StartPublish.Value : MaxDate),
                FilterSortOrder.PublishedDescending => DateDescending(page.StartPublish.HasValue ? page.StartPublish.Value : MinDate),
                FilterSortOrder.Rank => "-",
                _ => throw new NotImplementedException()
            };
        }

        static string DateAscending(DateTime date) => date.ToString("yyyyMMddhhmm");

        static string DateDescending(DateTime date) => DateTime.Now.Subtract(date).TotalMinutes.ToString("0000000");

        static string PeerSortOrder(PageData page)
        {
            int pagePeerSortOrder = page.GetPropertyValue<int>(MetaDataProperties.PagePeerOrder);

            // Current sort order is signed int, might be negative.
            // So start at 1/2 uint.MAX, and add the signed int value.
            uint peerOrder;
            if (pagePeerSortOrder < 0)
                peerOrder = 2147483647 - (uint)Math.Abs(pagePeerSortOrder);
            else
                peerOrder = 2147483647 + (uint)pagePeerSortOrder;

            return peerOrder.ToString("X8");
        }

        static string Normalize(string input)
            => new string(input.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    }
}
