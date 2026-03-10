namespace Munientry.Client.Shared.Models;

/// <summary>
/// Immutable representation of a judicial officer resolved at session start.
/// Mirrors the Python JudicialOfficer(Person) dataclass used throughout the legacy app.
/// </summary>
public sealed record JudicialOfficer(
    string FirstName,
    string LastName,
    string OfficerType)
{
    public string FullName => $"{FirstName} {LastName}";
    public bool IsVisiting => OfficerType == "Visiting Judge";
}
