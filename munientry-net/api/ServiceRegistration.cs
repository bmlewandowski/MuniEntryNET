using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Munientry.Api.Middleware;
using Munientry.Api.Options;
using Munientry.Api.Services;

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
        services.Configure<DailyListOptions>(configuration.GetSection("DailyList"));
        services.Configure<SchedulingOptions>(configuration.GetSection("Scheduling"));

        // ── Exception handling (item #5) ──────────────────────────────────────────
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        // ── Audit middleware (item #20) ───────────────────────────────────────
        services.AddTransient<AuditMiddleware>();

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
