"""
Preprocesses DOCX templates from resources/Templates into api/Templates.
Converts Jinja2-style {{ variable }} tokens (which Word splits across XML runs)
into single-run {Placeholder} tokens for use with DocumentFormat.OpenXml.

Run once from the repo root (or poc folder root):
    python api/Templates/prepare_templates.py
"""

import zipfile
import re
import shutil
import io
import os

# ---------------------------------------------------------------------------
# Token maps: Jinja2 token inner text (normalized, spaces removed) → placeholder
# ---------------------------------------------------------------------------
TIME_TO_PAY_TOKENS = {
    "case_number": "CaseNumber",
    "entry_date": "EntryDate",
    "defendant.first_name": "DefendantFirstName",
    "defendant.last_name": "DefendantLastName",
    "appearance_date": "AppearanceDate",
    "judicial_officer.first_name": "JudicialOfficerFirstName",
    "judicial_officer.last_name": "JudicialOfficerLastName",
    "judicial_officer.officer_type": "JudicialOfficerType",
}

TRIAL_TO_COURT_TOKENS = {
    "case_number": "CaseNumber",
    "defendant.first_name": "DefendantFirstName",
    "defendant.last_name": "DefendantLastName",
    "assigned_judge": "AssignedJudge",
    "trial_to_court.date": "TrialToCourtDate",
    "trial_to_court.time": "TrialToCourtTime",
    "trial_to_court.location": "AssignedCourtroom",
    "judicial_officer.first_name": "JudicialOfficerFirstName",
    "judicial_officer.last_name": "JudicialOfficerLastName",
    "judicial_officer.officer_type": "JudicialOfficerType",
    "interpreter_language": "LanguageRequired",
}

DRIVING_PRIVILEGES_TOKENS = {
    "case_number": "CaseNumber",
    "defendant.first_name": "DefendantFirstName",
    "defendant.last_name": "DefendantLastName",
    "defendant.license_number": "DefendantLicenseNumber",
    "defendant.birth_date": "DefendantBirthDate",
    "defendant.address": "DefendantAddress",
    "defendant.city": "DefendantCity",
    "defendant.state": "DefendantState",
    "defendant.zipcode": "DefendantZipcode",
    "suspension_type": "SuspensionType",
    "suspension_start_date": "SuspensionStartDate",
    "suspension_end_date": "SuspensionEndDate",
    "bmv_cases": "BmvCases",
    "related_traffic_case_number": "RelatedTrafficCaseNumber",
    "employer.privileges_type": "EmployerPrivilegesType",
    "employer.name": "EmployerName",
    "employer.address": "EmployerAddress",
    "employer.city": "EmployerCity",
    "employer.state": "EmployerState",
    "employer.zipcode": "EmployerZipcode",
    "employer.driving_days": "EmployerDrivingDays",
    "employer.driving_hours": "EmployerDrivingHours",
    "employer.other_conditions": "EmployerOtherConditions",
    "additional_information_text": "AdditionalInformationText",
}

TEMPLATES = [
    ("Time_To_Pay_Template.docx", TIME_TO_PAY_TOKENS),
    ("Trial_To_Court_Hearing_Notice_Template.docx", TRIAL_TO_COURT_TOKENS),
    ("Driving_Privileges_Template.docx", DRIVING_PRIVILEGES_TOKENS),
]

RESOURCES_DIR = os.path.abspath(
    os.path.join(os.path.dirname(__file__), "source")
)
OUTPUT_DIR = os.path.abspath(os.path.dirname(__file__))


def normalize_token_key(inner: str) -> str:
    """Remove all whitespace from inside a {{ }} token to get the canonical key."""
    return re.sub(r"\s+", "", inner)


def replace_jinja_tokens_in_xml(xml: str, token_map: dict) -> str:
    """
    Replaces Jinja2 {{ variable }} tokens in document XML.

    Word splits tokens across multiple <w:t> runs. We handle this by:
    1. Collecting the full text of each <w:p> paragraph by concatenating all <w:t> values.
    2. If the combined text contains {{ ... }}, rebuild the paragraph using a
       'run merger': scan through <w:r> runs and buffer any that span a token
       boundary, then emit a single merged run with the replacement text.
    """
    W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"

    def get_run_text(run_xml: str) -> str:
        parts = re.findall(r"<w:t[^>]*>(.*?)</w:t>", run_xml, re.DOTALL)
        return "".join(parts)

    def get_first_rpr(run_xml: str) -> str:
        m = re.search(r"<w:rPr>.*?</w:rPr>", run_xml, re.DOTALL)
        return m.group(0) if m else ""

    def make_run(rpr: str, text: str) -> str:
        if not text:
            return ""
        safe = text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
        space = ' xml:space="preserve"' if text != text.strip() else ""
        return f"<w:r>{rpr}<w:t{space}>{safe}</w:t></w:r>"

    def process_paragraph(para_xml: str) -> str:
        # Check if this paragraph contains any Jinja2 tokens at all
        all_text_parts = re.findall(r"<w:t[^>]*>(.*?)</w:t>", para_xml, re.DOTALL)
        combined = "".join(all_text_parts)

        if "{{" not in combined:
            return para_xml  # nothing to do

        # Normalise: collapse spaces inside {{ ... }} for matching
        def norm_combined(text):
            return re.sub(
                r"\{\{(.*?)\}\}",
                lambda m: "{{" + normalize_token_key(m.group(1)) + "}}",
                text,
                flags=re.DOTALL,
            )

        normalised = norm_combined(combined)

        # Apply token replacements on the normalised combined text
        result = normalised
        for inner_key, placeholder in token_map.items():
            result = result.replace("{{" + inner_key + "}}", "{" + placeholder + "}")

        if result == normalised:
            return para_xml  # tokens present but none in our map — leave alone

        # --- Rebuild the paragraph ---
        # Keep <w:pPr> unchanged
        ppr_match = re.search(r"<w:pPr>.*?</w:pPr>", para_xml, re.DOTALL)
        ppr = ppr_match.group(0) if ppr_match else ""

        # Extract all <w:r> runs (simple, non-recursive)
        runs = re.findall(r"<w:r[ >].*?</w:r>", para_xml, re.DOTALL)

        # Merge all run text, tracking positions
        run_texts = [get_run_text(r) for r in runs]
        rpr_from_first = get_first_rpr(runs[0]) if runs else ""

        # The combined text after merging all runs
        full_text = "".join(run_texts)

        # Normalise combined text the same way we did above
        norm_full = norm_combined(full_text)
        # Apply replacements
        replaced_text = norm_full
        for inner_key, placeholder in token_map.items():
            replaced_text = replaced_text.replace(
                "{{" + inner_key + "}}", "{" + placeholder + "}"
            )

        if replaced_text == full_text:
            return para_xml

        # --- Emit new runs: keep non-token literal segments, single run per token ---
        # Split on {Placeholder} boundaries so we can reuse formatting from the
        # original first run for the replacement runs.
        parts = re.split(r"(\{[A-Za-z]+\})", replaced_text)
        new_runs = []
        for part in parts:
            if not part:
                continue
            if re.fullmatch(r"\{[A-Za-z]+\}", part):
                # Token replacement — use first run's rPr
                new_runs.append(make_run(rpr_from_first, part))
            else:
                # Literal text — use first run's rPr for now (preserves basic style)
                new_runs.append(make_run(rpr_from_first, part))

        # Reconstruct the paragraph
        p_tag = re.match(r"<w:p( [^>]*)?>", para_xml)
        open_tag = p_tag.group(0) if p_tag else "<w:p>"
        return open_tag + ppr + "".join(new_runs) + "</w:p>"

    # Split XML into paragraphs, process each, reassemble
    def replace_para(m):
        return process_paragraph(m.group(0))

    return re.sub(r"<w:p[ >].*?</w:p>", replace_para, xml, flags=re.DOTALL)


def process_template(src_path: str, dst_path: str, token_map: dict) -> None:
    print(f"Processing: {os.path.basename(src_path)}")
    with open(src_path, "rb") as f:
        src_bytes = f.read()

    in_buf = io.BytesIO(src_bytes)
    out_buf = io.BytesIO()

    with zipfile.ZipFile(in_buf, "r") as zin:
        with zipfile.ZipFile(out_buf, "w", compression=zipfile.ZIP_DEFLATED) as zout:
            for item in zin.infolist():
                data = zin.read(item.filename)
                if item.filename == "word/document.xml":
                    xml = data.decode("utf-8")
                    xml = replace_jinja_tokens_in_xml(xml, token_map)
                    # Strip any remaining Jinja2 control blocks ({%...%})
                    xml = re.sub(r'\{%[^%]*?%\}', '', xml, flags=re.DOTALL)
                    data = xml.encode("utf-8")
                zout.writestr(item, data)

    with open(dst_path, "wb") as f:
        f.write(out_buf.getvalue())
    print(f"  → Written to: {dst_path}")


def main():
    for filename, token_map in TEMPLATES:
        src = os.path.join(RESOURCES_DIR, filename)
        dst = os.path.join(OUTPUT_DIR, filename)
        if not os.path.exists(src):
            print(f"  SKIPPING (not found): {src}")
            continue
        process_template(src, dst, token_map)
    print("Done.")


if __name__ == "__main__":
    main()
