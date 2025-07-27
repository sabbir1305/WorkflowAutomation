namespace Workflow.UI.Models;

public class WorkflowModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TriggerType { get; set; } = string.Empty;
}