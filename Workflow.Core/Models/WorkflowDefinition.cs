using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Models
{
    public record WorkflowDefinition
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string TriggerType { get; init; }
        public List<ActionDefinition> Actions { get; init; } = [];
    }
}
