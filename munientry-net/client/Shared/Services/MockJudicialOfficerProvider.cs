using Microsoft.Extensions.Configuration;
using Munientry.Client.Shared.Models;

namespace Munientry.Client.Shared.Services;

/// <summary>
/// Development/testing implementation of IJudicialOfficerProvider.
/// Resolves the judicial officer from the MockUser key in appsettings.json —
/// no Entra ID authentication required.
///
/// To use:
///   1. Set "MockUser" in wwwroot/appsettings.json to any key in the
///      "Staff:JudicialOfficers" section (e.g. "judge_1", "visiting_judge").
///   2. Ensure MockJudicialOfficerProvider is registered in Program.cs
///      (it is the default until Entra ID is enabled).
///
/// To switch to real Entra ID auth:
///   Follow the ENTRA ID steps in Program.cs and register
///   EntraIdJudicialOfficerProvider in place of this class.
/// </summary>
public sealed class MockJudicialOfficerProvider : IJudicialOfficerProvider
{
    private readonly IConfiguration _config;

    public MockJudicialOfficerProvider(IConfiguration config)
    {
        _config = config;
    }

    public Task<JudicialOfficer?> GetCurrentOfficerAsync()
    {
        var mockKey = _config["MockUser"] ?? string.Empty;
        var officer = Resolve(mockKey);
        return Task.FromResult(officer);
    }

    public bool IsVisitingJudge()
    {
        var mockKey = _config["MockUser"] ?? string.Empty;
        var isVisiting = _config.GetValue<bool>($"Staff:JudicialOfficers:{mockKey}:IsVisiting");
        return isVisiting;
    }

    /// <summary>Looks up a staff key in the Staff:JudicialOfficers config section.</summary>
    private JudicialOfficer? Resolve(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;

        var section = _config.GetSection($"Staff:JudicialOfficers:{key}");
        if (!section.Exists()) return null;

        var firstName   = section["FirstName"]   ?? string.Empty;
        var lastName    = section["LastName"]    ?? string.Empty;
        var officerType = section["OfficerType"] ?? string.Empty;

        return new JudicialOfficer(firstName, lastName, officerType);
    }
}
