using Munientry.Shared.Dtos;

namespace Munientry.Api.Services;

public class CompetencyEvaluationService : DocxServiceBase<CompetencyEvaluationDto>, ICompetencyEvaluationService
{
    protected override string TemplatePath { get; } =
        TemplateResolver.Get("Competency_Evaluation_Template.docx");

    protected override Dictionary<string, object> BuildTokens(CompetencyEvaluationDto dto) => new()
    {
        ["case_number"]                   = dto.CaseNumber ?? "",
        ["defendant.first_name"]          = dto.DefendantFirstName ?? "",
        ["defendant.last_name"]           = dto.DefendantLastName ?? "",
        ["plea_trial_date"]               = dto.EntryDate?.ToString("MMMM dd, yyyy") ?? "",
        ["treatment_type"]                = dto.EvaluatorName ?? "",
        ["final_pretrial.date"]           = dto.CompetencyHearingDate?.ToString("MMMM dd, yyyy") ?? "",
        ["final_pretrial.time"]           = dto.CompetencyHearingType ?? "",
        ["jury_trial.date"]               = dto.EvaluationDate?.ToString("MMMM dd, yyyy") ?? "",
        ["judicial_officer.first_name"]   = dto.JudicialOfficerFirstName ?? "",
        ["judicial_officer.last_name"]    = dto.JudicialOfficerLastName ?? "",
        ["judicial_officer.officer_type"] = dto.JudicialOfficerType ?? "",
    };
}
