using System.Diagnostics.CodeAnalysis;

namespace BlendInteractive.Solr.Optimizely
{
    public record SolrIdentifier(string ContentId, string Repository, string Language)
    {
        public override string ToString() => $"{Repository}://{Language}/{ContentId}";

        public static bool TryParse(string? id, [NotNullWhen(true)] out SolrIdentifier? identifier)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                identifier = default;
                return false;
            }

            var split = id.Split("://");
            if (split.Length != 2)
            {
                identifier = default;
                return false;
            }

            var remainder = split[1].Split('/');
            if (remainder.Length != 2)
            {
                identifier = default;
                return false;
            }

            identifier = new SolrIdentifier(remainder[1], split[0], remainder[0]);
            return true;
        }
    }
}
