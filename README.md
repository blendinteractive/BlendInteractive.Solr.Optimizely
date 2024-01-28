# Blend Interactive's Solr integration

This repo contains a basic Solr integration implementation for Optimizely CMS 12. It is a ground-up rewrite of our CMS 11 implementation. If you are migrating from the CMS 11 version, not all features have been migrated over and there may be some gaps to fill.

While Issues and Pull Requets are welcome, Blend makes no guarantees about maintaining this library or responding quickly.

## Installation and Usage

To install:

1. Install the `BlendInteractive.Solr.Optimizely` nuget package.
2. Optionally, create a custom Solr document type with the custom fields you wish to use. (Note: if using the default document, use `SolrDocument` instead of your custom class.)

```csharp
    public class CustomSolrDocument : SolrDocument
    {
        [SolrField("headline_s")]
        public string? Headline { get; set; }
    }
```

3. Add Solr services to your start up class.

```csharp
        services
            .AddSolrNet<CustomSolrDocument>("http://localhost:8983/solr/example") // Adding a core for the `CustomSolrDocument` document type.
            .AddOptimizelySolr<CustomSolrDocument>() // Adds the Optimizely related services
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();
```

4. Add the synchronization events for your Solr document type:

```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IContentEvents contentEvents)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapContent();
        });

        contentEvents.SynchronizeSolr<CustomSolrDocument>(); // Adds content events to synchronize Solr documents
    }
```

5. Configure your models to add their content to the full-text field for searching by implementing `IHaveFullText`:

```csharp
    public T AddContent<T>(T builder) where T : FullTextBuilder
        => builder
            .AddText(Headline)
            .AddHtml(Body)
            .AddContentArea(Content);
```

6. Optionally, you can set any custom fields by implementing `IHaveCustomSolrDocument<>` on your models:

```csharp
    void IHaveCustomSolrDocument<CustomSolrDocument>.ApplyTo(CustomSolrDocument doc)
    {
        doc.Headline = Headline;
    }
```

7. Optionally, you can exclude content from the index by implementing `IExcludeFromSearch` on your model:

```csharp
    public bool ExcludeFromSearch => true;
```

8. Finally, create a new `OptimizelyQuery<>` of your document type, and execute the search with the `SolrSearchService<>` service:

```csharp
    // Query the full text and Headline, boosting headline a bit.
    var query = new OptimizelyQuery<CustomSolrDocument>(q, 
        new QueryField(SolrDocument.FieldNames.Text, 1),
        new QueryField(CustomSolrDocument.CustomFieldNames.Headline, 2)
    );

    // Only published, public pages, in the current language, under the current site.
    query
        .WithDefaults()
        .InLanguage(currentContent.Language.Name)
        .PagesOnly()
        .WithinSite(SiteDefinition.Current.Id);

    // Pagination
    page = Math.Max(0, page);
    var limit = new Limit(page * PageWeight, PageWeight);

    var results = searchService.ExecuteQuery(query, limit);
```

## Reindex all content

If you'd like a scheduled job to reindex all content, add a new scheduled job to your project and extend `AbstractSolrReindexScheduledJob<>` for your document type. No logic is needed in the scheduled job. For example:

```csharp
    [ScheduledPlugIn(
        DisplayName = "Reindex all content in Solr",
        DefaultEnabled = true,
        GUID = "eee2bc6d-2be1-4d35-8b1c-166c39bf4671")]
    public class SolrReindexScheduledJob : AbstractSolrReindexScheduledJob<CustomSolrDocument>
    {
        public SolrReindexScheduledJob(SolrSynchronizationUtility<CustomSolrDocument> synchronizationUtility, ISiteDefinitionRepository siteDefinitionRepository, IContentLoader contentLoader, ILanguageBranchRepository languageBranchRepository) : base(synchronizationUtility, siteDefinitionRepository, contentLoader, languageBranchRepository)
        {
        }
    }
```

