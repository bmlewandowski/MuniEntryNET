using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Munientry.Poc.Client.Shared.Models;

namespace Munientry.Poc.Client.Shared
{
    /// <summary>
    /// Generic base component for all criminal/probation/driving entry forms.
    /// Eliminates ~60 lines of boilerplate duplicated across forms by centralising:
    ///   - OnParametersSetAsync / _loadedCaseNumber guard
    ///   - LoadCaseDataAsync via CaseSearchService
    ///   - HandleValidSubmit (posts Model to ApiEndpoint, handles DOCX download or text result)
    ///   - IsSubmitting / IsLoadingCase / ErrorMessage / SubmitResult state
    ///   - CloseOrCancel / ClearFields helpers
    /// Forms only need to override PopulateFromCaseAsync and ApiEndpoint.
    /// </summary>
    public abstract class FormPageBase<TModel> : ComponentBase where TModel : class, new()
    {
        // ── Parameters ──────────────────────────────────────────────────────────
        [Parameter] public string? CaseNumber { get; set; }

        // ── State ────────────────────────────────────────────────────────────────
        protected TModel Model { get; set; } = new();
        protected bool IsSubmitting { get; set; }
        protected bool IsLoadingCase { get; set; }
        protected string? ErrorMessage { get; set; }
        protected string? SubmitResult { get; set; }

        // ── Injected services ────────────────────────────────────────────────────
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected ICriminalFormApiClient ApiClient { get; set; } = default!;
        [Inject] protected CaseSearchService CaseSearchService { get; set; } = default!;
        /// <summary>Available to subclasses that need the API base URL for custom HTTP calls (e.g. GET requests).</summary>
        [Inject] protected ApiHelper ApiHelper { get; set; } = default!;

        // ── Abstract / virtual seam points ───────────────────────────────────────

        /// <summary>
        /// The relative API endpoint to POST the form model to (e.g. "api/diversionplea").
        /// Return an empty string for forms whose submit is not yet implemented.
        /// </summary>
        protected virtual string ApiEndpoint => string.Empty;

        /// <summary>
        /// Called after case data is fetched. Override to map CaseSearchResultDto fields
        /// onto the form's Model. Default implementation is a no-op.
        /// </summary>
        protected virtual Task PopulateFromCaseAsync(List<CaseSearchResultDto> results)
            => Task.CompletedTask;

        // ── Lifecycle ────────────────────────────────────────────────────────────
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
        /// that use a different API endpoint (e.g. DrivingPrivileges → /api/drivingcase/).
        /// </summary>
        protected virtual async Task LoadCaseDataAsync(string caseNumber)
        {
            IsLoadingCase = true;
            ErrorMessage = null;
            try
            {
                var results = await CaseSearchService.SearchCaseAsync(caseNumber);
                if (results == null || results.Count == 0) return;
                await PopulateFromCaseAsync(results);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load case: {ex.Message}";
            }
            finally
            {
                IsLoadingCase = false;
            }
        }

        // ── Submit ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Default submit handler: POSTs Model to ApiEndpoint. If the response is a DOCX,
        /// triggers a browser download via JS interop; otherwise shows a success message.
        /// Forms that need custom submit logic can override this method.
        /// </summary>
        protected virtual async Task HandleValidSubmit()
        {
            if (string.IsNullOrEmpty(ApiEndpoint))
            {
                SubmitResult = "Submit not yet implemented for this form.";
                return;
            }

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
                    ErrorMessage = $"Error: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        // ── Navigation helpers ───────────────────────────────────────────────────
        protected virtual void CloseOrCancel() => Navigation.NavigateTo("/");

        protected virtual void ClearFields()
        {
            Model = new TModel();
            _loadedCaseNumber = null;
            ErrorMessage = null;
            SubmitResult = null;
        }
    }
}
