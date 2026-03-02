using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Munientry.Api.Services;
using Munientry.Api.Tests.Fakes;

namespace Munientry.Api.Tests.Infrastructure
{
    /// <summary>
    /// Shared <see cref="WebApplicationFactory{TEntryPoint}"/> for the entire MuniEntry test suite.
    ///
    /// <para>
    /// All seven SQL-querying services are replaced with in-memory fakes so the tests run
    /// without a SQL Server connection.  Every other service (DOCX generation, validation,
    /// middleware, health checks) remains real, keeping the integration tests meaningful.
    /// </para>
    ///
    /// <para>Usage in a test class:</para>
    /// <code>
    /// public class MyApiTests : IClassFixture&lt;MuniEntryWebApplicationFactory&gt;
    /// {
    ///     private readonly HttpClient _client;
    ///     public MyApiTests(MuniEntryWebApplicationFactory factory) =>
    ///         _client = factory.CreateClient();
    /// }
    /// </code>
    ///
    /// <para>
    /// If a test class needs a behaviorally different fake for a specific scenario it can
    /// still call <c>factory.WithWebHostBuilder(...)</c> to override individual services,
    /// but this should be the exception rather than the rule.
    /// </para>
    /// </summary>
    public class MuniEntryWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // ── Replace each SQL-querying service with its in-memory fake ────────────
                //    The DOCX-generation services are pure template operations and are left
                //    as-is — they don't touch the database.
                ReplaceService<ICaseSearchService,     FakeCaseSearchService>(services);
                ReplaceService<IDailyListService,      FakeDailyListService>(services);
                ReplaceService<ICaseDocketService,     FakeCaseDocketService>(services);
                ReplaceService<IDrivingCaseService,    FakeDrivingCaseService>(services);
                ReplaceService<IEventReportService,    FakeEventReportService>(services);
                ReplaceService<IFtaReportService,      FakeFtaReportService>(services);
                ReplaceService<INotGuiltyReportService, FakeNotGuiltyReportService>(services);
            });
        }

        // ── Helper ────────────────────────────────────────────────────────────────────
        private static void ReplaceService<TService, TImplementation>(IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            var existing = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
            if (existing is not null)
                services.Remove(existing);
            services.AddScoped<TService, TImplementation>();
        }
    }
}
