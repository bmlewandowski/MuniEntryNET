using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Munientry.Client.Shared.Services;
using Munientry.Shared.Dtos;

namespace Munientry.Client.Shared
{
    /// <summary>
    /// Generic base component for all entry forms (criminal, probation, driving, admin, civil, notices, scheduling).
    /// Eliminates ~60 lines of boilerplate duplicated across forms by centralising:
    ///   - OnParametersSetAsync / _loadedCaseNumber guard
    ///   - LoadCaseDataAsync via CaseSearchApiClient
    ///   - HandleValidSubmit (posts Model to ApiEndpoint, handles DOCX download or text result)
    ///   - IsSubmitting / IsLoadingCase / ErrorMessage / SubmitResult state
    ///   - CloseOrCancel / ClearFields helpers
    /// Forms only need to override PopulateFromCaseAsync and ApiEndpoint.
    /// Forms that need a custom GET call (e.g. DrivingPrivileges) should inject
    /// ApiHelper directly rather than relying on the base class.
    /// </summary>
    public abstract class FormPageBase<TModel> : ComponentBase where TModel : class, new()
    {
        // -- Parameters ----------------------------------------------------------
        [Parameter] public string? CaseNumber { get; set; }

        // -- State ----------------------------------------------------------------
        protected TModel Model { get; set; } = new();
        protected bool IsSubmitting { get; set; }
        protected bool IsLoadingCase { get; set; }
        protected string? ErrorMessage { get; set; }
        protected string? SubmitResult { get; set; }

        // -- Injected services ----------------------------------------------------
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected IEntryFormApiClient ApiClient { get; set; } = default!;
        [Inject] protected CaseSearchApiClient CaseSearch { get; set; } = default!;
        [Inject] protected JudicialOfficerSession OfficerSession { get; set; } = default!;

        // -- Abstract / virtual seam points ---------------------------------------

        /// <summary>
        /// The relative API endpoint to POST the form model to (e.g. "diversionplea").
        /// Return an empty string for forms whose submit is not yet implemented.
        /// </summary>
        protected virtual string ApiEndpoint => string.Empty;

        /// <summary>
        /// Called after case data is fetched. Override to map CaseSearchResultDto fields
        /// onto the form's Model. Default implementation is a no-op.
        /// </summary>
        protected virtual Task PopulateFromCaseAsync(List<CaseSearchResultDto> results)
            => Task.CompletedTask;

        // -- Lifecycle ------------------------------------------------------------
        private string? _loadedCaseNumber;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(CaseNumber) && CaseNumber != _loadedCaseNumber)
            {
                _loadedCaseNumber = CaseNumber;
                await LoadCaseDataAsync(CaseNumber);
            }
        }

        /// <summary>
        /// Fetches case data and calls PopulateFromCaseAsync. Override completely for forms
        /// that use a different API endpoint (e.g. DrivingPrivileges ? /api/drivingcase/).
        /// </summary>
        protected virtual async Task LoadCaseDataAsync(string caseNumber)
        {
            IsLoadingCase = true;
            ErrorMessage = null;
            try
            {
                var results = await CaseSearch.SearchCaseAsync(caseNumber);
                if (results == null || results.Count == 0) return;
                await PopulateFromCaseAsync(results);
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to load case data. Please try again.";
            }
            finally
            {
                IsLoadingCase = false;
            }
        }

        // -- Submit ---------------------------------------------------------------

        /// <summary>
        /// Default submit handler: stamps the judicial officer onto the model, then POSTs
        /// to ApiEndpoint. If the response is a DOCX, triggers a browser download via JS
        /// interop; otherwise shows a success message.
        /// Forms that need custom submit logic can override this method.
        /// </summary>
        protected virtual async Task HandleValidSubmit()
        {
            if (string.IsNullOrEmpty(ApiEndpoint))
            {
                SubmitResult = "Submit not yet implemented for this form.";
                return;
            }

            StampJudicialOfficer();

            IsSubmitting = true;
            ErrorMessage = null;
            SubmitResult = null;
            try
            {
                var response = await ApiClient.PostAsync(ApiEndpoint, Model);
                if (response.IsSuccessStatusCode)
                {
                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
                    if (contentType.Contains("wordprocessingml") || contentType.Contains("octet-stream"))
                    {
                        var bytes = await response.Content.ReadAsByteArrayAsync();
                        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                                       ?? "entry.docx";
                        await JS.InvokeVoidAsync(
                            "downloadFile",
                            fileName,
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                            bytes);
                    }
                    else
                    {
                        SubmitResult = "Entry saved successfully.";
                    }
                }
                else
                {
                    ErrorMessage = await ReadErrorMessageAsync(response);
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        // -- Error helpers --------------------------------------------------------

        /// <summary>
        /// Reads the RFC 7807 Problem Details <c>detail</c> field from a non-success
        /// API response. Falls back to a generic message if the body is absent or unparseable.
        /// Never surfaces raw exception messages or internal server paths to the UI.
        /// </summary>
        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("detail", out var detail))
                        return detail.GetString() ?? "An error occurred. Please try again.";
                }
            }
            catch (Exception)
            {
                // Ignore parse failures — fall through to the generic message.
            }
            return "An error occurred. Please try again.";
        }

        // -- Navigation helpers ---------------------------------------------------
        protected virtual void CloseOrCancel() => Navigation.NavigateTo("/");

        protected virtual void ClearFields()
        {
            Model = new TModel();
            _loadedCaseNumber = null;
            ErrorMessage = null;
            SubmitResult = null;
        }

        // -- Judicial Officer stamping -------------------------------------------

        /// <summary>
        /// Copies the session's JudicialOfficer into any of the judicial officer
        /// properties that the DTO exposes.  Uses reflection so that every form
        /// gets the correct officer without any per-form code.
        ///
        /// Two DTO patterns are handled:
        ///   Split-field pattern (most forms):
        ///     JudicialOfficerFirstName, JudicialOfficerLastName, JudicialOfficerType
        ///   Single-field pattern (FiscalJournalEntryDto):
        ///     JudicialOfficer  →  officer.FullName ("FirstName LastName")
        ///
        /// Mirrors what the legacy Python base builder did:
        ///   self.entry.judicial_officer = self.dialog.judicial_officer
        /// </summary>
        protected void StampJudicialOfficer()
        {
            var officer = OfficerSession.JudicialOfficer;
            if (officer is null || Model is null) return;

            var type = typeof(TModel);

            // Split-field pattern — used by ~35 DTOs
            SetIfExists(type, "JudicialOfficerFirstName", officer.FirstName);
            SetIfExists(type, "JudicialOfficerLastName",  officer.LastName);
            SetIfExists(type, "JudicialOfficerType",      officer.OfficerType);

            // Single-field pattern — used by FiscalJournalEntryDto
            SetIfExists(type, "JudicialOfficer", officer.FullName);
        }

        private void SetIfExists(Type type, string propertyName, string value)
        {
            var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop is not null && prop.CanWrite)
                prop.SetValue(Model, value);
        }
    }
}
