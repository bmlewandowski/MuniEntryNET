namespace Munientry.Api;

/// <summary>
/// Sanitizes string data loaded from the AuthorityCourt SQL Server database before it is
/// surfaced to the Blazor client or written to DOCX templates.
///
/// Mirrors <c>munientry.data.data_cleaners</c> from the legacy Python application exactly,
/// including the word-substitution dictionary and capitalisation rules. The same four
/// operations are available:
///   <list type="bullet">
///     <item><see cref="CleanLastName"/> — removes spaces around hyphens in last names</item>
///     <item><see cref="CleanOffenseName"/> — normalises raw offense strings from the DB</item>
///     <item><see cref="CleanDefenseCounselName"/> — normalises attorney name strings</item>
///     <item><see cref="CleanStatuteName"/> — strips trailing '*' from statute codes</item>
///   </list>
/// </summary>
internal static class CourtDataCleaner
{
    // -----------------------------------------------------------------
    // Offense substitution table
    // Mirrors OFFENSE_CLEAN_DICT (MappingProxyType) in data_cleaners.py.
    // Keys are upper-case tokens as they appear in the DB.
    // Values are the display strings; an empty string means the token is
    // stripped from the output (e.g. degree codes: M1, M2, UCM…).
    // -----------------------------------------------------------------
    private static readonly IReadOnlyDictionary<string, string> OffenseCleanDict =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "UCM",          ""              },
            { "M1",           ""              },
            { "M2",           ""              },
            { "M3",           ""              },
            { "M4",           ""              },
            { "MM",           ""              },
            { "PETTY",        ""              },
            { "(UCM)",        ""              },
            { "ACDA",         "ACDA"          },
            { "FTY",          "FTY"           },
            { "FTY/ROW",      "FTY / ROW"     },
            { "DUS",          "DUS"           },
            { "OVI",          "OVI"           },
            { "BMV",          "BMV"           },
            { "OBMV",         "BMV"           },
            { "FRA",          "FRA"           },
            { "OL",           "OL"            },
            { "OMVI",         "OVI"           },
            { "FRA/JUDGMENT", "FRA / Judgment"},
            { "OR",           "/"             },
            { "W/O",          "Without"       },
            { "A",            "a"             },
            { "TO",           "to"            },
            { "SUSP",         "Suspension"    },
            { "-",            ""              },
            { "OF",           "of"            },
            { "IN",           "in"            },
            { "AND",          "and"           },
        };

    // -----------------------------------------------------------------
    // Defense counsel substitution table
    // Mirrors DEFENSE_COUNSEL_CLEAN_DICT in data_cleaners.py.
    // Preserves Roman numerals like "III" that Title Case would corrupt.
    // -----------------------------------------------------------------
    private static readonly IReadOnlyDictionary<string, string> CounselCleanDict =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "III", "III" },
        };

    // -----------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------

    /// <summary>
    /// Removes spaces around a hyphen in hyphenated last names.
    /// e.g. <c>"SMITH - JONES"</c> → <c>"SMITH-JONES"</c>
    /// Mirrors <c>clean_last_name()</c> in data_cleaners.py.
    /// </summary>
    public static string CleanLastName(string? lastName)
    {
        if (string.IsNullOrEmpty(lastName)) return string.Empty;
        return lastName.Replace(" - ", "-");
    }

    /// <summary>
    /// Normalises an offense description loaded from the database.
    /// Each whitespace-delimited token is looked up in <see cref="OffenseCleanDict"/>;
    /// tokens not in the dictionary are Title-cased via <see cref="Capitalize"/>.
    /// Tokens that map to an empty string (degree codes, bare hyphens) are dropped.
    /// Mirrors <c>clean_offense_name()</c> in data_cleaners.py.
    /// </summary>
    public static string CleanOffenseName(string? offense)
    {
        if (string.IsNullOrWhiteSpace(offense)) return string.Empty;

        var words = offense.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cleanWords = words
            .Select(w => OffenseCleanDict.TryGetValue(w, out var replacement) ? replacement : Capitalize(w))
            .Where(w => w.Length > 0);

        return string.Join(' ', cleanWords).TrimEnd();
    }

    /// <summary>
    /// Normalises a defense counsel name loaded from the database.
    /// Each whitespace-delimited token is Title-cased, with exceptions for
    /// Roman numerals and other tokens in <see cref="CounselCleanDict"/>.
    /// Mirrors <c>clean_defense_counsel_name()</c> in data_cleaners.py.
    /// </summary>
    public static string CleanDefenseCounselName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;

        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cleanWords = words
            .Select(w => CounselCleanDict.TryGetValue(w, out var replacement) ? replacement : Capitalize(w));

        return string.Join(' ', cleanWords).TrimEnd();
    }

    /// <summary>
    /// Strips a trailing <c>*</c> from a statute code.
    /// e.g. <c>"4511.19*"</c> → <c>"4511.19"</c>
    /// Mirrors <c>clean_statute_name()</c> in data_cleaners.py.
    /// </summary>
    public static string CleanStatuteName(string? statute)
    {
        if (string.IsNullOrEmpty(statute)) return string.Empty;
        return statute.TrimEnd('*');
    }

    // -----------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------

    /// <summary>
    /// Upper-cases the first character and lower-cases the remainder of a token.
    /// Equivalent to Python's <c>str.capitalize()</c>.
    /// </summary>
    private static string Capitalize(string word) =>
        word.Length == 0
            ? word
            : char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant();
}
