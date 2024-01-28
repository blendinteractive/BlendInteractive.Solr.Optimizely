using BlendInteractive.Solr.Optimizely;
using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Example.Opti.Models;
using SolrNet;

namespace Example.Opti;

public class Startup
{
    private readonly IWebHostEnvironment _webHostingEnvironment;

    public Startup(IWebHostEnvironment webHostingEnvironment)
    {
        _webHostingEnvironment = webHostingEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_webHostingEnvironment.IsDevelopment())
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

            services.Configure<SchedulerOptions>(options => options.Enabled = false);
        }

        services
            .AddSolrNet<CustomSolrDocument>("http://localhost:8983/solr/example")
            .AddOptimizelySolr<CustomSolrDocument>()
            .AddCmsAspNetIdentity<ApplicationUser>()
            .AddCms()
            .AddAdminUserRegistration()
            .AddEmbeddedLocalization<Startup>();
    }

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

        contentEvents.SynchronizeSolr<CustomSolrDocument>();
    }
}
