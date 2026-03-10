using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Munientry.Api.Middleware;
using Munientry.Api.Options;
using Munientry.Api.Services;
using Polly;
using Polly.Retry;

namespace Munientry.Api;

/// <summary>
/// Extension method that registers all API services with the DI container.
/// Keeps Program.cs to a minimal bootstrap — add new services here, not there.
/// </summary>
internal static class ServiceRegistration
{
    internal static IServiceCollection AddMuniEntryServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ── Strongly-typed options ────────────────────────────────────────────
        // AuthorityCourtOptions — the key name and null-guard are written once here;
        // all 7 SQL-querying services inject IOptions<AuthorityCourtOptions> instead
        // of IConfiguration, keeping the coupling to the config graph in one place.
        services.Configure<AuthorityCourtOptions>(opts =>
            opts.ConnectionString = configuration.GetConnectionString("AuthorityCourt")
                ?? throw new InvalidOperationException("Missing connection string 'AuthorityCourt'."));
        services.Configure<SchedulingOptions>(configuration.GetSection("Scheduling"));

        // ── Exception handling (item #5) ──────────────────────────────────────────
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        // ── Audit middleware (item #20) ───────────────────────────────────────
        services.AddTransient<AuditMiddleware>();

        // ── SQL transient-fault resilience pipeline (item #13) ───────────────
        // Shared by all 7 SqlServiceBase subclasses via ResiliencePipelineProvider<string>.
        // Retries up to 3 times on transient SqlException or TimeoutException with
        // exponential back-off (300 ms base, jitter) and a 30-second per-attempt timeout.
        // The full open→execute→read cycle is inside each ExecuteAsync callback so a fresh
        // SqlConnection is created on every retry — no stale connection is reused.
        services.AddResiliencePipeline("sql-transient", pipeline =>
            pipeline
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay            = TimeSpan.FromMilliseconds(300),
                    BackoffType      = DelayBackoffType.Exponential,
                    UseJitter        = true,
                    ShouldHandle     = args => args.Outcome switch
                    {
                        { Exception: SqlException { IsTransient: true } } => PredicateResult.True(),
                        { Exception: TimeoutException }                   => PredicateResult.True(),
                        _                                                 => PredicateResult.False(),
                    },
                })
                .AddTimeout(TimeSpan.FromSeconds(30)));

        // ── Interface-bound (mockable / testable) ────────────────────────────
        services.AddScoped<ICaseSearchService, CaseSearchService>();
        services.AddScoped<IDailyListService, DailyListService>();
        services.AddScoped<IDrivingCaseService, DrivingCaseService>();
        services.AddScoped<ICaseDocketService, CaseDocketService>();
        services.AddScoped<IEventReportService, EventReportService>();
        services.AddScoped<IFtaReportService, FtaReportService>();
        services.AddScoped<INotGuiltyReportService, NotGuiltyReportService>();
        services.AddScoped<ICommunityControlTermsService, CommunityControlTermsService>();
        services.AddScoped<ILeapSentencingService, LeapSentencingService>();
        services.AddScoped<IFiscalJournalEntryService, FiscalJournalEntryService>();

        // ── Interface-bound (item #11 resolved) ──────────────────────────────
        services.AddScoped<IDrivingPrivilegesService, DrivingPrivilegesService>();
        services.AddScoped<IDenyPrivilegesPermitRetestService, DenyPrivilegesPermitRetestService>();
        services.AddScoped<INoticesFreeformCivilService, NoticesFreeformCivilService>();
        services.AddScoped<ITrialToCourtNoticeService, TrialToCourtNoticeService>();
        services.AddScoped<IFinalJuryNoticeService, FinalJuryNoticeService>();
        services.AddScoped<IBondHearingService, BondHearingService>();
        services.AddScoped<IProbationViolationBondService, ProbationViolationBondService>();
        services.AddScoped<IBondModificationRevocationService, BondModificationRevocationService>();
        services.AddScoped<ITimeToPayOrderService, TimeToPayOrderService>();
        services.AddScoped<IJurorPaymentService, JurorPaymentService>();
        services.AddScoped<ICommunityControlTermsNoticesService, CommunityControlTermsNoticesService>();
        services.AddScoped<ICommunityServiceSecondaryService, CommunityServiceSecondaryService>();
        services.AddScoped<IFineOnlyPleaService, FineOnlyPleaService>();
        services.AddScoped<IArraignmentContinuanceService, ArraignmentContinuanceService>();
        services.AddScoped<INotGuiltyPleaService, NotGuiltyPleaService>();
        services.AddScoped<INotGuiltyAppearBondSpecialService, NotGuiltyAppearBondSpecialService>();
        services.AddScoped<IAppearOnWarrantNoPleaService, AppearOnWarrantNoPleaService>();
        services.AddScoped<ILeapAdmissionPleaService, LeapAdmissionPleaService>();
        services.AddScoped<ILeapAdmissionAlreadyValidService, LeapAdmissionAlreadyValidService>();
        services.AddScoped<ILeapValidSentencingService, LeapValidSentencingService>();
        services.AddScoped<ISentencingOnlyAlreadyPleadService, SentencingOnlyAlreadyPleadService>();
        services.AddScoped<IPleaOnlyFutureSentencingService, PleaOnlyFutureSentencingService>();
        services.AddScoped<IDiversionDialogService, DiversionDialogService>();
        services.AddScoped<IDiversionPleaService, DiversionPleaService>();
        services.AddScoped<ICivilFreeformEntryService, CivilFreeformEntryService>();
        services.AddScoped<IGeneralNoticeOfHearingService, GeneralNoticeOfHearingService>();
        services.AddScoped<ISchedulingEntryService, SchedulingEntryService>();
        services.AddScoped<IJailCcPleaService, JailCcPleaService>();
        services.AddScoped<ICriminalSealingEntryService, CriminalSealingEntryService>();
        services.AddScoped<ICompetencyEvaluationService, CompetencyEvaluationService>();
        services.AddScoped<IFailureToAppearService, FailureToAppearService>();
        services.AddScoped<ICriminalFreeformEntryService, CriminalFreeformEntryService>();
        services.AddScoped<ITrialSentencingService, TrialSentencingService>();

        return services;
    }
}
