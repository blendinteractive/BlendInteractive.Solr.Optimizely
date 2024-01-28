using System.Text.RegularExpressions;

namespace BlendInteractive.Solr
{
    public static class FullTextBuilderExtensions
    {
        static readonly Regex StripHtmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        static readonly Regex WhitespaceCollapseRegex = new Regex("\\s{2-1000}", RegexOptions.Compiled);

        public static T AddText<T>(this T builder, string? text) where T : FullTextBuilder
        {
            if (string.IsNullOrWhiteSpace(text))
                return builder;

            string collapsed = WhitespaceCollapseRegex.Replace(text, "");
            if (string.IsNullOrWhiteSpace(collapsed))
                return builder;

            if (builder.Buffer.Length > 0)
                builder.Buffer.Append(' ');

            builder.Buffer.Append(collapsed);
            return builder;
        }

        public static T AddHtml<T>(this T builder, string? rawHtml) where T : FullTextBuilder
        {
            if (string.IsNullOrWhiteSpace(rawHtml))
                return builder;

            string stripped = StripHtmlRegex.Replace(rawHtml, "");

            return builder.AddText(stripped);
        }
    }
}
