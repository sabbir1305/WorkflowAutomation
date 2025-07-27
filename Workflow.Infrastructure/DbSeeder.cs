using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Workflow.Core.Models;

namespace Workflow.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(WorkflowDbContext context)
        {
            if (!await context.Workflows.AnyAsync())
            {
                var workflowId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var actionId = Guid.Parse("22222222-2222-2222-2222-222222222222");

                var workflow = new WorkflowDefinition
                {
                    Id = workflowId,
                    Name = "Sample Workflow",
                    TriggerType = "Manual",
                    Actions = new List<ActionDefinition>
                    {
                        new ActionDefinition
                        {
                            Id = actionId,
                            ActionType = "Email",
                            Parameters = new List<ActionParameter>
                            {
                                new ActionParameter { Key = "to", Value = "user@example.com" },
                                new ActionParameter { Key = "subject", Value = "Hello" }
                            }
                        }
                    }
                };

                context.Workflows.Add(workflow);
                await context.SaveChangesAsync();
            }
        }
    }
}
