using SolrNet;

namespace BlendInteractive.Solr
{
    public class SolrOperationsManager<TDoc> : IDisposable where TDoc : ISolrDocument
    {
        private readonly ISolrOperations<TDoc> solr;
        private int pendingOperations;

        private int operationsPerCommit = 10;

        public int OperationsPerCommit
        {
            get => operationsPerCommit;
            set
            {
                operationsPerCommit = value;
                CommitIfReady();
            }
        }

        public SolrOperationsManager(ISolrOperations<TDoc> solr)
        {
            this.solr = solr;
        }

        public virtual void Add(TDoc doc)
        {
            solr.Delete(new SolrQueryByField(Constants.IdentifierFieldName, doc.Id));
            solr.AddWithBoost(doc, doc.Boost);

            pendingOperations++;
            CommitIfReady();
        }

        public virtual void Delete(ISolrQuery query)
        {
            solr.Delete(query);
            pendingOperations++;
            CommitIfReady();
        }

        protected virtual void CommitIfReady()
        {
            if (pendingOperations >= operationsPerCommit)
            {
                Commit();
            }
        }

        public virtual void Commit()
        {
            if (pendingOperations > 0)
            {
                solr.Commit();
                pendingOperations = 0;
            }
        }

        public void Dispose()
        {
            Commit();
        }
    }
}
