using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow.Core.Models
{
    public record ActionDefinition
    {
        public required string ActionType { get; init; }
        public Dictionary<string, string> Parameters { get; init; } = new();
    }
}
