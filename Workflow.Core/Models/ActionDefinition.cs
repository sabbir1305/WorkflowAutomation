using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Models
{
    public class ActionParameter
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public record ActionDefinition
    {
        public Guid Id { get; init; }
        public required string ActionType { get; init; }
        public List<ActionParameter> Parameters { get; init; } = new();
    }
}
