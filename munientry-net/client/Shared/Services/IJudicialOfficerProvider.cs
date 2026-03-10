using Munientry.Client.Shared.Models;

namespace Munientry.Client.Shared.Services;

/// <summary>
/// Resolves the current judicial officer from the authentication context.
/// 
/// Two concrete implementations exist:
///   MockJudicialOfficerProvider  — reads MockUser from appsettings.json (no auth required).
///   EntraIdJudicialOfficerProvider (future) — resolves from the MSAL claims principal.
/// 
/// Swap registrations in Program.cs to switch between them.
/// </summary>
public interface IJudicialOfficerProvider
{
    /// <summary>
    /// Returns the judicial officer for the currently authenticated (or mocked) user,
    /// or null if the user cannot be mapped to a known officer.
    /// </summary>
    Task<JudicialOfficer?> GetCurrentOfficerAsync();

    /// <summary>
    /// Returns true when the current user should be treated as a visiting judge
    /// and must supply their name interactively before continuing.
    /// </summary>
    bool IsVisitingJudge();
}
