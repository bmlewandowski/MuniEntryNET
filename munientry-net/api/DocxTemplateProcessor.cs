using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Munientry.Api;

/// <summary>
/// Fills Jinja2-style DOCX templates directly — no preprocessing step required.
/// </summary>
/// <remarks>
/// <para><strong>How it works</strong></para>
/// <para>
/// Microsoft Word splits a single <c>{{ variable }}</c> token across multiple
/// <c>&lt;w:r&gt;</c> (run) elements in the underlying XML whenever the user pauses
/// typing, changes formatting mid-token, or Word applies spell-check/autocorrect.
/// Because of this fragmentation a simple string search can never find a complete token.
/// </para>
/// <para>
/// This class processes each <c>&lt;w:p&gt;</c> paragraph independently:
/// <list type="number">
///   <item>Concatenates the text of all <c>&lt;w:t&gt;</c> elements in the paragraph.</item>
///   <item>Normalises whitespace inside <c>{{ }}</c> tokens (Word may insert extra spaces).</item>
///   <item>Applies token → value substitution on the combined text.</item>
///   <item>Strips Jinja2 control blocks (<c>{% if/elif/endif %}</c>, <c>{% for/endfor %}</c>).</item>
///   <item>Rebuilds the paragraph as a single <c>&lt;w:r&gt;</c> run, reusing the first
///         run's <c>&lt;w:rPr&gt;</c> character-formatting block and the original
///         <c>&lt;w:pPr&gt;</c> paragraph-formatting block.</item>
/// </list>
/// </para>
/// <para>
/// Replicates the behaviour of the legacy <c>prepare_templates.py</c> Python preprocessing
/// script. Source template files (<c>api/Templates/source/*.docx</c>) contain raw Jinja2
/// tokens and are filled at request time; no offline preprocessing step is needed.
/// </para>
/// <para>
/// Processes all relevant XML parts: document body, headers, footers, footnotes, and
/// endnotes (<c>word/*.xml</c>). Structural parts (styles, settings, fonts, themes,
/// numbering) are copied byte-for-byte without modification.
/// </para>
///
/// <para><strong>Formatting preservation</strong></para>
/// <para>
/// The following formatting is fully preserved regardless of whether a paragraph contains tokens:
/// <list type="bullet">
///   <item>Paragraph-level formatting: indentation, spacing, alignment, tab stops
///         (<c>&lt;w:pPr&gt;</c> is never modified).</item>
///   <item>Page margins, section layout, headers, and footers
///         (styles.xml and settings.xml are not processed).</item>
///   <item>All paragraphs that contain no <c>{{ }}</c> or <c>{% %}</c> tokens are
///         copied byte-for-byte — their XML is completely untouched.</item>
/// </list>
/// </para>
/// <para>
/// The following formatting is subject to the same limitations as the legacy Python script,
/// because both approaches consolidate runs into one:
/// <list type="bullet">
///   <item><strong>Mixed character formatting within a token paragraph</strong> (e.g. one
///         word bold, another italic inside the same paragraph as a token) — all runs
///         inherit the first run's <c>&lt;w:rPr&gt;</c> after consolidation. This is
///         acceptable in court form templates where token-containing paragraphs are
///         uniformly styled plain text.</item>
///   <item><strong><c>&lt;w:tab&gt;</c> or <c>&lt;w:br&gt;</c> elements inside a
///         token's runs</strong> — only <c>&lt;w:t&gt;</c> text is extracted; inline
///         tab/break elements within those specific runs are dropped. Tab stops defined
///         at paragraph level in <c>&lt;w:pPr&gt;</c> are unaffected and still apply.</item>
///   <item><strong>Jinja2 <c>{% if %}</c> branches</strong> — only the first branch is
///         kept; conditional formatting variations in later branches are discarded.
///         This matches the Python behaviour.</item>
/// </list>
/// </para>
/// <para>
/// In practice these limitations do not affect the MuniEntry court form templates because
/// token-containing paragraphs (case number, names, dates) are uniformly formatted
/// plain text with paragraph-level tab stops.
/// </para>
/// </remarks>
public static class DocxTemplateProcessor
{
    // Template bytes are immutable after the first request — cache them once per path.
    // Key is the absolute path (via Path.GetFullPath) so relative-path variants collapse
    // to the same entry. byte[] is never mutated: ZipArchive reads it but each call
    // wraps the cached array in a new MemoryStream.
    private static readonly ConcurrentDictionary<string, byte[]> _templateCache = new();

    // Word XML parts that contain only structural/style data — skip them
    private static readonly string[] SkipPartSuffixes =
    [
        "/styles.xml", "/settings.xml", "/fontTable.xml",
        "/webSettings.xml", "/numbering.xml", "/theme/theme1.xml",
    ];

    /// <summary>
    /// Fills a DOCX template, supporting both flat string tokens and
    /// <c>{%tc for item in list %}</c> table-row loops.
    /// <para>
    /// Values whose type is <see cref="string"/> are substituted as plain tokens.
    /// Values whose type is <c>IEnumerable&lt;Dictionary&lt;string,string&gt;&gt;</c>
    /// drive table-row cloning: every <c>&lt;w:tr&gt;</c> that contains
    /// <c>{%tc for varName in listName %}</c> is replaced by one cloned row per item,
    /// with <c>{{ varName.field }}</c> tokens substituted from that item's dictionary.
    /// </para>
    /// </summary>
    public static byte[] FillTemplate(string templatePath, Dictionary<string, object> values)
    {
        var bytes = _templateCache.GetOrAdd(
            Path.GetFullPath(templatePath),
            static p => File.ReadAllBytes(p));
        using var ms = new MemoryStream(bytes, writable: false);
        return FillTemplate(ms, values);
    }

    /// <inheritdoc cref="FillTemplate(string,Dictionary{string,object})"/>
    public static byte[] FillTemplate(Stream templateStream, Dictionary<string, object> values)
    {
        // Read the entire stream so we can safely seek / re-open as a ZIP
        using var inMs = new MemoryStream();
        templateStream.CopyTo(inMs);
        inMs.Position = 0;

        var outMs = new MemoryStream();

        using (var zin  = new ZipArchive(inMs,  ZipArchiveMode.Read,   leaveOpen: true))
        using (var zout = new ZipArchive(outMs, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var entry in zin.Entries)
            {
                using var inStream  = entry.Open();
                var outEntry = zout.CreateEntry(entry.FullName, CompressionLevel.Optimal);
                using var outStream = outEntry.Open();

                if (ShouldProcessXml(entry.FullName))
                {
                    using var reader = new StreamReader(inStream, Encoding.UTF8);
                    var xml = reader.ReadToEnd();
                    xml = ProcessXml(xml, values);
                    var xmlBytes = Encoding.UTF8.GetBytes(xml);
                    outStream.Write(xmlBytes, 0, xmlBytes.Length);
                }
                else
                {
                    inStream.CopyTo(outStream);
                }
            }
        }

        return outMs.ToArray();
    }

    /// <summary>Backward-compatible overload — flat string tokens only.</summary>
    public static byte[] FillTemplate(string templatePath, Dictionary<string, string> values)
        => FillTemplate(templatePath, values.ToDictionary(kv => kv.Key, kv => (object)kv.Value));

    /// <summary>Backward-compatible overload — flat string tokens only.</summary>
    public static byte[] FillTemplate(Stream templateStream, Dictionary<string, string> values)
        => FillTemplate(templateStream, values.ToDictionary(kv => kv.Key, kv => (object)kv.Value));

    // -------------------------------------------------------------------------
    // Internal helpers
    // -------------------------------------------------------------------------

    private static bool ShouldProcessXml(string entryName)
    {
        if (!entryName.StartsWith("word/") || !entryName.EndsWith(".xml"))
            return false;
        foreach (var suffix in SkipPartSuffixes)
            if (entryName.EndsWith(suffix))
                return false;
        return true;
    }

    private static string ProcessXml(string xml, Dictionary<string, object> values)
    {
        // Step 1: expand {%tc for varName in listName %} table-row loops.
        // This must run before paragraph processing so the cloned rows are
        // already in place when paragraph-level token substitution runs.
        xml = ProcessTableRowLoops(xml, values);

        // Step 2: extract flat string values for paragraph-level substitution.
        var stringValues = values
            .Where(kv => kv.Value is string)
            .ToDictionary(kv => kv.Key, kv => (string)kv.Value!);

        // Process each <w:p> paragraph independently.
        // The paragraph boundary is where run-fragmentation is contained.
        xml = Regex.Replace(
            xml,
            @"<w:p[ >].*?</w:p>",
            m => ProcessParagraph(m.Value, stringValues),
            RegexOptions.Singleline);

        // Final sweep: remove any Jinja control tags or tokens that weren't in the map
        xml = Regex.Replace(xml, @"\{%.*?%\}",      "",  RegexOptions.Singleline);
        xml = Regex.Replace(xml, @"\{\{[^{}<>]*?\}\}", "", RegexOptions.Singleline);
        return xml;
    }

    /// <summary>
    /// Finds every <c>&lt;w:tr&gt;</c> row that contains a
    /// <c>{%tc for varName in listName %}</c> / <c>{%tc endfor %}</c> pair
    /// and replaces it with one cloned row per item in the named list.
    /// The loop variable tokens (<c>{{ varName.field }}</c>) in each cloned
    /// row are substituted via <see cref="ProcessParagraph"/> before the
    /// control tags are stripped.
    /// </summary>
    private static string ProcessTableRowLoops(string xml, Dictionary<string, object> values)
    {
        return Regex.Replace(
            xml,
            @"<w:tr[ >].*?</w:tr>",
            m => ExpandTableRow(m.Value, values),
            RegexOptions.Singleline);
    }

    private static string ExpandTableRow(string rowXml, Dictionary<string, object> values)
    {
        // Concatenate all <w:t> text in the row (may be fragmented across runs)
        var textMatches = Regex.Matches(rowXml, @"<w:t\b[^>]*>(.*?)</w:t>", RegexOptions.Singleline);
        var rowText = string.Concat(textMatches.Select(m => m.Groups[1].Value));

        // Collapse whitespace for reliable pattern matching
        var normalised = Regex.Replace(rowText, @"\s+", " ");

        // Detect {%tc for varName in listName %}
        var forMatch = Regex.Match(normalised, @"\{%tc\s+for\s+(\w+)\s+in\s+(\w+)\s*%\}");
        if (!forMatch.Success) return rowXml;

        var varName  = forMatch.Groups[1].Value; // e.g. "charge"
        var listName = forMatch.Groups[2].Value; // e.g. "charges_list"

        if (!values.TryGetValue(listName, out var listObj) ||
            listObj is not IEnumerable<Dictionary<string, string>> items)
            return ""; // No data — remove the template row entirely

        var sb = new StringBuilder();
        foreach (var item in items)
        {
            // Build a per-item substitution dictionary: "charge.offense" => "...", ...
            var itemValues = item.ToDictionary(
                kv => $"{varName}.{kv.Key}",
                kv => kv.Value,
                StringComparer.OrdinalIgnoreCase);

            // Clone the row and apply paragraph-level token substitution
            var cloned = Regex.Replace(
                rowXml,
                @"<w:p[ >].*?</w:p>",
                pm => ProcessParagraph(pm.Value, itemValues),
                RegexOptions.Singleline);

            // Strip the {%tc ... %} control tags from the cloned row
            cloned = Regex.Replace(cloned, @"\{%tc[^%]*%\}", "", RegexOptions.Singleline);

            sb.Append(cloned);
        }
        return sb.ToString();
    }

    private static string ProcessParagraph(string paraXml, Dictionary<string, string> values)
    {
        // Collect raw XML content of all <w:t> elements (this is what a human reads)
        var textMatches = Regex.Matches(paraXml, @"<w:t\b[^>]*>(.*?)</w:t>", RegexOptions.Singleline);
        var combined    = string.Concat(textMatches.Select(m => m.Groups[1].Value));

        // Quick exit: nothing Jinja-related in this paragraph
        if (!combined.Contains("{{") && !combined.Contains("{%"))
            return paraXml;

        // Normalise whitespace inside {{ }} tokens — Word sometimes inserts spaces
        // around the token keys when the user typed them directly in a Word document.
        var normalized = NormalizeTokenWhitespace(combined);

        // Apply replacements: {{ key }} -> XML-escaped value
        var result = normalized;
        foreach (var (key, rawValue) in values)
        {
            var normalizedKey = CollapseWhitespace(key);
            result = result.Replace("{{" + normalizedKey + "}}", XmlEscape(rawValue));
        }

        // Strip Jinja control blocks ({% if %}...{% elif %}...{% endif %}, etc.)
        result = StripJinjaBlocks(result);

        // If nothing changed, leave the original XML untouched
        if (result == combined)
            return paraXml;

        // ---- Rebuild the paragraph ----------------------------------------
        // Keep <w:pPr> (paragraph formatting) as-is.
        var ppr = Regex.Match(paraXml, @"<w:pPr>.*?</w:pPr>", RegexOptions.Singleline) is { Success: true } pm
            ? pm.Value
            : string.Empty;

        // Use the first run's <w:rPr> (character formatting) for all rebuilt runs.
        // This preserves the dominant styling (font, size, bold, etc.) of the paragraph.
        var runs   = Regex.Matches(paraXml, @"<w:r[ >].*?</w:r>", RegexOptions.Singleline);
        var firstRpr = runs.Count > 0
            ? (Regex.Match(runs[0].Value, @"<w:rPr>.*?</w:rPr>", RegexOptions.Singleline) is { Success: true } rm
                ? rm.Value : string.Empty)
            : string.Empty;

        // Emit the entire result as a single run, preserving whitespace
        var newRun = MakeRun(firstRpr, result);

        var openTag = Regex.Match(paraXml, @"<w:p( [^>]*)?>") is { Success: true } pt
            ? pt.Value
            : "<w:p>";

        return openTag + ppr + newRun + "</w:p>";
    }

    /// <summary>
    /// Collapses all whitespace inside <c>{{ ... }}</c> tokens so that a token like
    /// <c>{{ defendant . first_name }}</c> matches the key <c>defendant.first_name</c>.
    /// </summary>
    private static string NormalizeTokenWhitespace(string text) =>
        Regex.Replace(
            text,
            @"\{\{(.*?)\}\}",
            m => "{{" + CollapseWhitespace(m.Groups[1].Value) + "}}",
            RegexOptions.Singleline);

    private static string CollapseWhitespace(string s) =>
        Regex.Replace(s, @"\s+", "");

    private static string XmlEscape(string value) =>
        value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    private static string MakeRun(string rpr, string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var safe  = XmlEscape(text);
        // xml:space="preserve" tells Word to keep leading/trailing whitespace
        var space = (text != text.Trim()) ? " xml:space=\"preserve\"" : string.Empty;
        return $"<w:r>{rpr}<w:t{space}>{safe}</w:t></w:r>";
    }

    /// <summary>
    /// Strips Jinja2 control blocks from plain text content:
    /// <list type="bullet">
    ///   <item><c>{% if %}...{% elif/else %}...{% endif %}</c> — keeps only the first branch</item>
    ///   <item><c>{% for %}...{% endfor %}</c> — keeps the loop body once</item>
    ///   <item>Remaining bare <c>{% %}</c> tags — removed</item>
    /// </list>
    /// This mirrors the <c>strip_jinja_blocks()</c> function in prepare_templates.py.
    /// </summary>
    private static string StripJinjaBlocks(string text)
    {
        // Keep first branch of if / elif / else / endif
        text = Regex.Replace(
            text,
            @"\{%-?\s*if\b.*?%\}.*?\{%-?\s*endif\s*-?%\}",
            m =>
            {
                var full  = m.Value;
                // Strip the opening {% if ... %} tag
                var inner = Regex.Replace(full, @"^\{%-?\s*if\b.*?%\}", string.Empty, RegexOptions.Singleline);
                // Trim at the first {% elif %} / {% else %} / {% endif %}
                var cut   = Regex.Match(inner, @"\{%-?\s*(?:elif|else|endif)\b");
                return cut.Success ? inner[..cut.Index].Trim() : inner.Trim();
            },
            RegexOptions.Singleline);

        // Keep for-loop body once (render one iteration)
        text = Regex.Replace(
            text,
            @"\{%-?\s*for\b.*?%\}.*?\{%-?\s*endfor\s*-?%\}",
            m =>
            {
                var full  = m.Value;
                var inner = Regex.Replace(full, @"^\{%-?\s*for\b.*?%\}", string.Empty, RegexOptions.Singleline);
                inner     = Regex.Replace(inner, @"\{%-?\s*endfor\s*-?%\}$", string.Empty);
                return inner.Trim();
            },
            RegexOptions.Singleline);

        // Remove any remaining bare {% %} control tags
        return Regex.Replace(text, @"\{%.*?%\}", string.Empty, RegexOptions.Singleline);
    }
}
