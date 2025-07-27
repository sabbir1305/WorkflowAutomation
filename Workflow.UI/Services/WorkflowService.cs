using System.Net.Http.Json;
using Workflow.UI.Models;

namespace Workflow.UI.Services;

public class WorkflowService
{
    private readonly HttpClient _http;

    public WorkflowService(IHttpClientFactory clientFactory)
    {
        _http = clientFactory.CreateClient("API");
    }

    public async Task<List<WorkflowModel>> GetWorkflowsAsync()
        => await _http.GetFromJsonAsync<List<WorkflowModel>>("/workflows") ?? new List<WorkflowModel>();

    public async Task<WorkflowModel?> GetWorkflowAsync(Guid id)
        => await _http.GetFromJsonAsync<WorkflowModel>($"/workflows/{id}");

    public async Task<bool> CreateWorkflowAsync(WorkflowModel workflow)
    {
        var response = await _http.PostAsJsonAsync("/workflows", workflow);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteWorkflowAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"/workflows/{id}");
        return response.IsSuccessStatusCode;
    }
}
