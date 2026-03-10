using Munientry.Client.Shared.Models;

namespace Munientry.Client.Shared.Services;

/// <summary>
/// Scoped per-user session state for the resolved judicial officer.
/// Replaces mainwindow.judicial_officer from the legacy Python application.
/// Registered as Scoped so each browser session gets its own instance.
/// </summary>
public sealed class JudicialOfficerSession
{
    public JudicialOfficer? JudicialOfficer { get; private set; }

    /// <summary>True once SetJudicialOfficer has been called successfully.</summary>
    public bool IsInitialized { get; private set; }

    /// <summary>Raised whenever the judicial officer changes (e.g. visiting judge confirmed).</summary>
    public event Action? OnChange;

    public void SetJudicialOfficer(JudicialOfficer officer)
    {
        JudicialOfficer = officer;
        IsInitialized = true;
        OnChange?.Invoke();
    }

    public void Clear()
    {
        JudicialOfficer = null;
        IsInitialized = false;
        OnChange?.Invoke();
    }
}
