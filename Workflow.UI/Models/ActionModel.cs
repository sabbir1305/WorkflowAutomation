namespace Workflow.UI.Models;

public class ActionModel
{
    public string Type { get; set; } = string.Empty;
    public List<ActionParameter> Parameters { get; set; } = new();
}
